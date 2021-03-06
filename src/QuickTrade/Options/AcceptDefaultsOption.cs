// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Options;

class AcceptDefaultsOption : Option<bool>
{
	public static AcceptDefaultsOption Instance { get; } = new();

	AcceptDefaultsOption() : base("--yes")
	{
		Description = Strings.Option_AcceptDefaults;

		AddAlias("-y");
	}
}
