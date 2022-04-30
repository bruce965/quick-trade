// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using QuickTrade.Configuration;
using QuickTrade.Options;
using QuickTrade.Utilities;
using System.CommandLine;
using System.Text.Json;

namespace QuickTrade.Commands;

class NewProjectCommand : Command
{
	public static NewProjectCommand Instance { get; } = new();

	NewProjectCommand() : base("project")
	{
		Description = Strings.Command_NewProject;

		AddOption(AcceptDefaultsOption.Instance);
		AddOption(ForceAcceptDefaultsOption.Instance);

		this.SetHandler(
			(Func<DirectoryInfo, bool, bool, Task>)Execute,
			ProjectDirectoryOption.Instance, AcceptDefaultsOption.Instance, ForceAcceptDefaultsOption.Instance);
	}

	static async Task Execute(DirectoryInfo projectDirectory, bool acceptDefaults, bool forceAcceptDefaults)
	{
		var project = await RunInteractive(projectDirectory, acceptDefaults, forceAcceptDefaults);
		if (project == null)
		{
			Console.Error.WriteLine(Strings.Operation_Aborted);
			return;
		}

		Console.WriteLine(Strings.Operation_Completed);
	}

	public static async Task<QuickTradeProject?> RunInteractive(DirectoryInfo projectDirectory, bool acceptDefaults, bool forceAcceptDefaults, CancellationToken cancellationToken = default)
	{
		Console.Error.WriteLine(Strings.Command_NewProject_Path, projectDirectory.FullName);

		if (!EnsureProjectDirectoryEmpty(projectDirectory, forceAcceptDefaults))
			return null;

		var config = await InitializeConfiguration(projectDirectory, acceptDefaults, forceAcceptDefaults, cancellationToken);
		if (config == null)
			return null;

		return config;
	}

	static bool EnsureProjectDirectoryEmpty(DirectoryInfo projectDirectory, bool forceAcceptDefaults)
	{
		if (projectDirectory.Exists && projectDirectory.EnumerateFileSystemInfos().Any())
		{
			var forceContinue = ConsoleInteractive.AskBoolean(Strings.Command_NewProject_NotEmpty, forceAcceptDefaults, forceAcceptDefaults);
			if (!forceContinue)
				return false;
		}

		return true;
	}

	static async ValueTask<QuickTradeProject?> InitializeConfiguration(DirectoryInfo projectDirectory, bool acceptDefaults, bool forceAcceptDefaults, CancellationToken cancellationToken = default)
	{
		QuickTradeProject? config = null;

		var configFile = new FileInfo(Path.Combine(projectDirectory.FullName, ProjectHelper.FileName));
		if (configFile.Exists)
		{
			try
			{
				using var stream = configFile.OpenRead();
				config = await ProjectHelper.LoadAsync(stream, cancellationToken);
			}
			catch (JsonException e)
			{
				Console.Error.WriteLine(Strings.Error_InvalidProjectFile, configFile.FullName, e.Message);

				var forceContinue = ConsoleInteractive.AskBoolean(Strings.Command_NewProject_OverwiteInvalidProject, forceAcceptDefaults, forceAcceptDefaults);
				if (!forceContinue)
					return null;
			}
		}

		if (config == null)
			config = new QuickTradeProject();

		using (var stream = configFile.OpenWrite())
		{
			await ProjectHelper.SaveAsync(stream, config, cancellationToken);

			stream.SetLength(stream.Position);
			await stream.FlushAsync(cancellationToken);
		}

		return config;
	}
}
