// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using Microsoft.Extensions.DependencyInjection;

namespace QuickTrade.Core.Plugins;

/// <summary>
/// Class for initializing services and middlewares provided by a plugin.
/// </summary>
public interface IPluginStartup
{
	/// <summary>
	/// Method to configure the plugin's services.
	/// </summary>
	/// <param name="services">The services collection to add the services to.</param>
	void ConfigureServices(IServiceCollection services) { }
}
