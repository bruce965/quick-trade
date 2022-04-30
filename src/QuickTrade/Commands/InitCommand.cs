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
		Description = Strings.Command_Init;

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
			Console.Error.WriteLine(Strings.Operation_Aborted);
			return;
		}

		var createPlugin = ConsoleInteractive.AskBoolean(Strings.Command_Init_Plugin, true, acceptDefaults);
		if (createPlugin)
			await NewPluginCommand.RunInteractive(projectDirectory, config, acceptDefaults, forceAcceptDefaults);

		Console.WriteLine(Strings.Operation_Completed);
	}
}
