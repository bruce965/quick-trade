// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using QuickTrade.Configuration;
using QuickTrade.Options;
using QuickTrade.Utilities;
using System.CommandLine;
using System.Text;
using System.Xml;

namespace QuickTrade.Commands;

class NewPluginCommand : Command
{
	public static NewPluginCommand Instance { get; } = new();

	NewPluginCommand() : base("plugin")
	{
		Description = "Create a new empty plugin";

		AddOption(AcceptDefaultsOption.Instance);
		AddOption(ForceAcceptDefaultsOption.Instance);

		this.SetHandler(
			(Func<DirectoryInfo, bool, bool, Task>)Execute,
			ProjectDirectoryOption.Instance, AcceptDefaultsOption.Instance, ForceAcceptDefaultsOption.Instance);
	}

	static async Task Execute(DirectoryInfo projectDirectory, bool acceptDefaults, bool forceAcceptDefaults)
	{
		var config = await ProjectHelper.LoadFromProjectDirectoryAsync(projectDirectory);
		if (config == null)
		{
			Console.WriteLine("Operation aborted.");
			return;
		}

		await RunInteractive(projectDirectory, config, acceptDefaults, forceAcceptDefaults);

		Console.WriteLine("Done!");
	}

	public static async Task RunInteractive(DirectoryInfo projectDirectory, QuickTradeProject config, bool acceptDefaults, bool forceAcceptDefaults)
	{
		var pluginName = ConsoleInteractive.AskString("Plugin name:", "MyPlugin", acceptDefaults);

		var projectPath = ConsoleInteractive.AskString("Project path:", $"{EnsureTrailingSlash(config.PluginsDir ?? ProjectHelper.DefaultPluginsDirName)}{pluginName}/{pluginName}.csproj", acceptDefaults);
		if (!projectPath.EndsWith(".csproj"))
			projectPath += ".csproj";

		// TODO: check if project already exists and react according to `forceAcceptDefaults`.

		var csprojFile = new FileInfo(Path.Combine(projectDirectory.FullName, projectPath));
		await CreateCsprojAsync(csprojFile, pluginName);

		var addToSolutionWithoutAsking = false;

		var slnFile = projectDirectory.EnumerateFiles("*.sln").FirstOrDefault();
		if (slnFile == null)
		{
			var createSolution = ConsoleInteractive.AskBoolean("Create a new solution?", true, acceptDefaults);
			if (createSolution)
			{
				var solutionName = ConsoleInteractive.AskString("Solution name:", "MyPlugins.sln", acceptDefaults);
				if (!solutionName.EndsWith(".sln"))
					solutionName += ".sln";

				slnFile = new FileInfo(Path.Combine(projectDirectory.FullName, solutionName));
				await CreateSlnAsync(slnFile);

				addToSolutionWithoutAsking = true;
			}
		}

		if (slnFile != null)
		{
			var addToSolution = addToSolutionWithoutAsking;
			if (!addToSolution)
			{
				// TODO: check if project is already in the solution and avoid adding it again.

				addToSolution = ConsoleInteractive.AskBoolean($"Add project to existing solution '{slnFile.Name}'?", true, acceptDefaults);
			}

			if (addToSolution)
				await AddToSlnAsync(slnFile, csprojFile, pluginName);
		}
	}

	static async Task CreateCsprojAsync(FileInfo csprojFile, string pluginName, CancellationToken cancellationToken = default)
	{
		csprojFile.Directory!.Create();

		using (var stream = csprojFile.OpenWrite())
		using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
		{
			var lines = new[]
			{
				@"<Project Sdk=""QuickTrade.Plugin.Sdk/1.0.0"">",  // TODO: place version number somewhere else.
				@"",
				@"  <PropertyGroup>",
				(pluginName == Path.GetFileNameWithoutExtension(csprojFile.FullName)) ? null : $@"    <AssemblyName>{EscapeXml(pluginName)}</AssemblyName>",
				@"    <TargetFramework>net6.0</TargetFramework>",
				@"    <ImplicitUsings>enable</ImplicitUsings>",
				@"    <Nullable>enable</Nullable>",
				@"  </PropertyGroup>",
				@"",
				@"</Project>",
			}.Where(line => line != null);

			foreach (var line in lines)
				await writer.WriteLineAsync(line);

			await writer.FlushAsync();
			await stream.FlushAsync();

			stream.SetLength(stream.Position);
		}

		var class1File = new FileInfo(Path.Combine(csprojFile.Directory!.FullName, "Class1.cs"));
		if (!class1File.Exists)
		{
			using (var stream = class1File.OpenWrite())
			using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
			{
				var lines = new[]
				{
					$"namespace {StringCasing.ToCSharpName(pluginName)};",
					"",
					"public class Class1",
					"{",
					"",
					"}",
				};

				foreach (var line in lines)
					await writer.WriteLineAsync(line);

				await writer.FlushAsync();
				await stream.FlushAsync();

				stream.SetLength(stream.Position);
			}
		}
	}

	static async Task CreateSlnAsync(FileInfo slnFile, CancellationToken cancellationToken = default)
	{
		using (var stream = slnFile.OpenWrite())
		using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
		{
			var lines = new[]
			{
				@"Microsoft Visual Studio Solution File, Format Version 12.00",
				@"# Visual Studio Version 16",
				@"VisualStudioVersion = 16.0.30114.105",
				@"MinimumVisualStudioVersion = 10.0.40219.1",
				@"Global",
				@"	GlobalSection(SolutionConfigurationPlatforms) = preSolution",
				@"		Debug|Any CPU = Debug|Any CPU",
				@"		Release|Any CPU = Release|Any CPU",
				@"	EndGlobalSection",
				@"	GlobalSection(SolutionProperties) = preSolution",
				@"		HideSolutionNode = FALSE",
				@"	EndGlobalSection",
				@"EndGlobal",
			};

			foreach (var line in lines)
				await writer.WriteLineAsync(line);

			await writer.FlushAsync();
			await stream.FlushAsync();

			stream.SetLength(stream.Position);
		}
	}

	static async Task AddToSlnAsync(FileInfo slnFile, FileInfo csprojFile, string pluginName, CancellationToken cancellationToken = default)
	{
		var projectGuid = Guid.NewGuid();
		var projectRelativePath = Path.GetRelativePath(slnFile.Directory!.FullName, csprojFile.FullName);

		var lines = await File.ReadAllLinesAsync(slnFile.FullName, cancellationToken);

		var beforeGlobal = lines
			.TakeWhile(line => line != "Global");

		var beforeProjectConfigurationPlatforms = lines
			.Skip(beforeGlobal.Count())
			.TakeWhile(line => line != "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution" && line != "EndGlobal");

		var hasProjectConfigurationPlatforms = lines
			.Skip(beforeGlobal.Count())
			.Skip(beforeProjectConfigurationPlatforms.Count())
			.FirstOrDefault() == "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution";

		var endOfProjectConfigurationPlatforms = !hasProjectConfigurationPlatforms ? Enumerable.Empty<string>() : lines
			.Skip(beforeGlobal.Count())
			.Skip(beforeProjectConfigurationPlatforms.Count())
			.TakeWhile(line => line != "\tEndGlobalSection" && line != "EndGlobal");

		var rest = lines
			.Skip(beforeGlobal.Count())
			.Skip(beforeProjectConfigurationPlatforms.Count())
			.Skip(endOfProjectConfigurationPlatforms.Count());

		var solutionConfigurationPlatforms = lines
			.SkipWhile(line => line != "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution")
			.Skip(1)
			.TakeWhile(line => line != "\tEndGlobalSection")
			.Select(line => line.TrimStart('\t'))
			.Select(line =>
			{
				var split = line.Split('=', 2, StringSplitOptions.TrimEntries);
				return (Left: split.Length > 0 ? split[0] : "", Right: split.Length > 1 ? split[1] : "");
			});

		using (var stream = slnFile.OpenWrite())
		using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
		{
			foreach (var line in beforeGlobal)
				await writer.WriteLineAsync(line);

			await writer.WriteLineAsync(@$"Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{pluginName}"", ""{projectRelativePath}"", ""{projectGuid.ToString("B").ToUpper()}""");
			await writer.WriteLineAsync("EndProject");

			foreach (var line in beforeProjectConfigurationPlatforms)
				await writer.WriteLineAsync(line);

			foreach (var line in endOfProjectConfigurationPlatforms)
				await writer.WriteLineAsync(line);

			if (!hasProjectConfigurationPlatforms)
				await writer.WriteLineAsync("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

			foreach (var (left, right) in solutionConfigurationPlatforms)
			{
				await writer.WriteLineAsync($"\t\t{projectGuid.ToString("B").ToUpper()}.{left}.ActiveCfg = {right}");
				await writer.WriteLineAsync($"\t\t{projectGuid.ToString("B").ToUpper()}.{left}.Build.0 = {right}");
			}

			if (!hasProjectConfigurationPlatforms)
				await writer.WriteLineAsync("\tEndGlobalSection");

			foreach (var line in rest)
				await writer.WriteLineAsync(line);

			await writer.FlushAsync();
			await stream.FlushAsync();

			stream.SetLength(stream.Position);
		}
	}

	static string EnsureTrailingSlash(string path)
	{
		if (!path.EndsWith(Path.DirectorySeparatorChar) && !path.EndsWith(Path.AltDirectorySeparatorChar))
			return path + Path.DirectorySeparatorChar;

		return path;
	}

	static string EscapeXml(string value)
	{
		var el = new XmlDocument().CreateElement("x");
		el.InnerText = value;

		var outerXml = el.OuterXml;
		return outerXml.Substring("<x>".Length, outerXml.Length - "<x></x>".Length);
	}
}
