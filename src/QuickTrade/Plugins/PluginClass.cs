// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using Microsoft.Extensions.DependencyInjection;
using QuickTrade.Core.Plugins;
using System.Reflection;

namespace QuickTrade.Plugins;

class PluginClass
{
	public class Instance : IPluginStartup
	{
		readonly PluginClass _plugin;
		readonly object _instance;

		public PluginClass Class => _plugin;

		internal Instance(PluginClass plugin)
		{
			_plugin = plugin;

			_instance = Activator.CreateInstance(plugin.Type) ?? throw new NotSupportedException("Activator returned null.");
		}

		public void ConfigureServices(IServiceCollection services)
		{
			// configure services if supported
			if (_instance is IPluginStartup startup)
				startup.ConfigureServices(services);
		}
	}

	readonly Type _type;

	public Type Type => _type;

	/// <inheritdoc cref="QuickTradePluginAttribute.Name"/>
	public string? PluginName
	{
		get
		{
			var attribute = _type.GetCustomAttribute<QuickTradePluginAttribute>();

			// if a name is explicitly set, use it
			if (attribute?.Name != null)
				return attribute.Name;

			// otherwise, generate a name from the type name
			var name = Type.FullName;

			// "MyPlugin.Startup" => "MyPlugin."
			// "MyNamespace.MyPluginStartup" => "MyNamespace.MyPlugin."
			if (name?.EndsWith("Startup") == true)
				name = name[..^"Startup".Length];

			// "MyPlugin." => "MyPlugin"
			// "MyNamespace.MyPlugin." => "MyNamespace.MyPlugin"
			return name?.TrimEnd('.');
		}
	}

	public PluginClass(Type type)
	{
		_type = type;
	}

	public Instance CreateInstance()
		=> new(this);
}
