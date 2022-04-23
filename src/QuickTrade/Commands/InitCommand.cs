// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using QuickTrade.Options;
using QuickTrade.Utilities;
using System.CommandLine;

namespace QuickTrade.Commands;

class InitCommand : Command
{
	public static InitCommand Instance { get; } = new();

	InitCommand() : base("init")
	{
		Description = "Create and configure a new project";

		AddOption(AcceptDefaultsOption.Instance);
		AddOption(ForceAcceptDefaultsOption.Instance);

		this.SetHandler(
			(Func<DirectoryInfo, bool, bool, Task>)Execute,
			ProjectDirectoryOption.Instance, AcceptDefaultsOption.Instance, ForceAcceptDefaultsOption.Instance);
	}

	static async Task Execute(DirectoryInfo projectDirectory, bool acceptDefaults, bool forceAcceptDefaults)
	{
		var config = await NewProjectCommand.RunInteractive(projectDirectory, acceptDefaults, forceAcceptDefaults);
		if (config == null)
		{
			Console.Error.WriteLine("Operation cancelled.");
			return;
		}

		if (config.PluginsDir != null)
			projectDirectory.CreateSubdirectory(config.PluginsDir);

		var createPlugin = ConsoleInteractive.AskBoolean("Do you want to create a custom plugin project?", true, acceptDefaults);
		if (createPlugin)
			await NewPluginCommand.RunInteractive(projectDirectory, config, acceptDefaults, forceAcceptDefaults);

		Console.WriteLine("Done!");
	}
}
