// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using QuickTrade.Core.Plugins;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace QuickTrade.Plugins;

class PluginAssembly
{
	readonly PluginLoadContext? _loadContext;

	public IReadOnlyList<PluginClass> Plugins { get; }

	public PluginAssembly(string dllPath, bool includeSymbols = false, bool copyToMemory = false)
	{
		_loadContext = new PluginLoadContext(dllPath, includeSymbols: includeSymbols, copyToMemory: copyToMemory);

		var assembly = _loadContext.LoadFromAssemblyPath(dllPath);

		Plugins = assembly.GetTypes()
			.Where(t => t.GetCustomAttribute<QuickTradePluginAttribute>() != null)
			.Select(t => new PluginClass(t))
			.ToList();
	}

	/// <summary>
	/// Check if an assembly contains plugins.
	/// </summary>
	/// <param name="dllPath"></param>
	/// <returns></returns>
	public static bool ContainsPlugins(string dllPath)
	{
		// TODO: the best thing would be to use reflection, but it's not supported yet. Or is it? https://github.com/dotnet/corefx/pull/33201

		[MethodImpl(MethodImplOptions.NoInlining)]
		static bool ContainsPlugins(string dllPath, out WeakReference weakRef)
		{
			// https://docs.microsoft.com/en-us/dotnet/standard/assembly/unloadability

			var loadContext = new PluginLoadContext(dllPath, copyToMemory: true, enableCollection: true);

			bool containsPlugins;
			try
			{
				var assembly = loadContext.LoadFromAssemblyPath(dllPath);

				containsPlugins = assembly.GetTypes().Any(t => t.GetCustomAttribute<QuickTradePluginAttribute>() != null);
			}
			catch (BadImageFormatException)
			{
				containsPlugins = false;
			}

			// unload assembly
			loadContext.Unload();

			weakRef = new WeakReference(loadContext);

			return containsPlugins;
		}

		var containsPlugins = ContainsPlugins(dllPath, out var weakRef);

		// collect everything, a few iterations may be necessary in case finalizers inside the assembly instantiate new objects
		for (var i = 0; i < 10; i++)
		{
			if (!weakRef.IsAlive)
				break;

			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		return containsPlugins;
	}
}
