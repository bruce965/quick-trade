// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Options;

class DebugLogOption : Option<bool>
{
	public static DebugLogOption Instance { get; } = new();

	DebugLogOption() : base("--verbose")
	{
		Description = Strings.Option_DebugLog;

		AddAlias("-v");
	}
}
