// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickTrade.Configuration;
using QuickTrade.Options;
using QuickTrade.Plugins;
using System.CommandLine;

namespace QuickTrade.Commands;

class StartCommand : Command
{
	public static StartCommand Instance { get; } = new();

	StartCommand() : base("start")
	{
		Description = Strings.Command_Start;

		AddOption(DebugLogOption.Instance);
		AddOption(DevelopmentModeOption.Instance);

		this.SetHandler(
			(Func<DirectoryInfo, bool, bool, Task>)Execute,
			ProjectDirectoryOption.Instance, DebugLogOption.Instance, DevelopmentModeOption.Instance);
	}

	static async Task Execute(DirectoryInfo projectDirectory, bool debugLog, bool developmentMode)
	{
		var project = await ProjectHelper.LoadFromProjectDirectoryAsync(projectDirectory);
		if (project == null)
		{
			Console.WriteLine(Strings.Operation_Aborted);
			return;
		}

		var builder = CreateHostBuilder(projectDirectory.FullName, debugLog, developmentMode);

		builder.ConfigureServices(services =>
		{
			using var provider = services.BuildServiceProvider();
			var logger = provider.GetRequiredService<ILogger<StartCommand>>();

			var plugins = LoadPlugins(logger, projectDirectory, project);

			foreach (var plugin in plugins)
			{
				logger.LogDebug("Configuring plugin: {Name}", plugin.Class.PluginName);

				plugin.ConfigureServices(services);
			}
		});

		var host = builder.Build();

		await host.RunAsync();
	}

	static IHostBuilder CreateHostBuilder(string projectDirectory, bool debugLog, bool developmentMode)
	{
		var builder = new HostBuilder();

		builder.UseEnvironment(developmentMode ? "Development" : "Production");

		builder.UseContentRoot(projectDirectory);

		builder.ConfigureAppConfiguration((hostingContext, config) =>
		{
			var projectFile = Path.Combine(projectDirectory, ProjectHelper.FileName);
			config.AddJsonFile(projectFile, optional: false, reloadOnChange: false);
		});

		builder.ConfigureLogging((hostingContext, logging) =>
		{
			logging.AddConsole();

			logging.SetMinimumLevel(debugLog ? LogLevel.Debug : LogLevel.Information);

			logging.Configure(options =>
			{
				options.ActivityTrackingOptions =
					ActivityTrackingOptions.SpanId |
					ActivityTrackingOptions.TraceId |
					ActivityTrackingOptions.ParentId;
			});
		});

		builder.UseDefaultServiceProvider((context, options) =>
		{
			if (context.HostingEnvironment.IsDevelopment())
			{
				options.ValidateScopes = true;
				options.ValidateOnBuild = true;
			}
		});

		return builder;
	}

	static IReadOnlyList<PluginClass.Instance> LoadPlugins(ILogger logger, DirectoryInfo projectDirectory, QuickTradeProject project)
	{
		var plugins = new List<PluginClass.Instance>();

		logger.LogDebug("Loading plugins...");

		foreach (var dllPath in EnumeratePluginAssemblies(projectDirectory, project))
		{
			if (!PluginAssembly.ContainsPlugins(dllPath))
			{
				logger.LogDebug("Skipping assembly which does not contain plugins: {Path}", dllPath);
				continue;
			}

			logger.LogDebug("Loading plugin assembly: {Path}", dllPath);

			var assembly = new PluginAssembly(dllPath);

			foreach (var plugin in assembly.Plugins)
			{
				logger.LogInformation("Loading plugin: {Name}", plugin.PluginName);

				var instance = plugin.CreateInstance();
				plugins.Add(instance);
			}
		}

		return plugins;
	}

	static IEnumerable<string> EnumeratePluginAssemblies(DirectoryInfo projectDirectory, QuickTradeProject project)
	{
		// TODO: it would be nice to match absolute paths as external, unlike in .gitignore files.

		var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
		foreach (var pattern in project.LoadPlugins)
		{
			if (pattern.StartsWith('!'))
				matcher.AddExclude(pattern[1..]);
			else
				matcher.AddInclude(pattern);
		}

		var plugins = matcher.Execute(new DirectoryInfoWrapper(projectDirectory));
		foreach (var match in plugins.Files)
		{
			var pluginAssembly = Path.Combine(projectDirectory.FullName, match.Path);
			yield return Path.GetFullPath(pluginAssembly);  // normalize directory separators
		}
	}
}
