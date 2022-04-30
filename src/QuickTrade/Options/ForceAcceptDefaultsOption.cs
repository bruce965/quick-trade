// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Options;

class ForceAcceptDefaultsOption : Option<bool>
{
	public static ForceAcceptDefaultsOption Instance { get; } = new();

	ForceAcceptDefaultsOption() : base("--force-yes")
	{
		Description = Strings.Option_ForceAcceptDefaults;
	}
}
