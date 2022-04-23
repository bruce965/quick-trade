// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

namespace QuickTrade.Configuration;

class QuickTradeProject
{
	public int Version { get; set; } = 1;

	public string? PluginsDir { get; set; } = "./plugins/";
}
