// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Commands;

class NewCommand : Command
{
	public static NewCommand Instance { get; } = new();

	NewCommand() : base("new")
	{
		Description = Strings.Command_New;

		AddCommand(NewProjectCommand.Instance);
		AddCommand(NewPluginCommand.Instance);
	}
}
