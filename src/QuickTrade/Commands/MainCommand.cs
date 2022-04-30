// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using QuickTrade.Options;
using System.CommandLine;

namespace QuickTrade.Commands;

class MainCommand : RootCommand
{
	public static MainCommand Instance { get; } = new();

	MainCommand()
	{
		Description = Strings.Command_Main;

		AddGlobalOption(ProjectDirectoryOption.Instance);

		AddCommand(InitCommand.Instance);
		AddCommand(NewCommand.Instance);
		AddCommand(StartCommand.Instance);
	}
}
