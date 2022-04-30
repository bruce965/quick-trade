// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

namespace QuickTrade.Configuration;

class QuickTradeProject
{
	public int Version { get; set; } = ProjectHelper.LatestVersion;

	public ICollection<string> LoadPlugins { get; set; } = new List<string>
	{
		$"{ProjectHelper.DefaultPluginsDirName}/*Plugin.dll",
	};
}
