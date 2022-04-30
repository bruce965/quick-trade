// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Options;

class DevelopmentModeOption : Option<bool>
{
	public static DevelopmentModeOption Instance { get; } = new();

	DevelopmentModeOption() : base("--dev")
	{
		Description = Strings.Option_DevelopmentMode;

		AddAlias("-D");
	}
}
