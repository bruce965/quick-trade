// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.Reflection;
using System.Runtime.Loader;

namespace QuickTrade.Plugins;

class PluginLoadContext : AssemblyLoadContext
{
	readonly AssemblyDependencyResolver _resolver;
	readonly bool _includeSymbols;
	readonly bool _copyToMemory;

	public PluginLoadContext(
		string dllPath,
		bool copyToMemory = false,
		bool includeSymbols = false,
		bool enableCollection = false)
		: base(enableCollection)
	{
		_resolver = new AssemblyDependencyResolver(dllPath);
		_includeSymbols = includeSymbols;
		_copyToMemory = copyToMemory;
	}

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
		if (assemblyPath == null)
			return null;

		return LoadFromAssemblyPath(assemblyPath);
	}

	/// <inheritdoc cref="AssemblyLoadContext.LoadFromAssemblyPath(string)"/>
	public new Assembly LoadFromAssemblyPath(string assemblyPath)
	{
		Console.WriteLine("Loading: {0}", assemblyPath);

		if (_copyToMemory)
		{
			MemoryStream? symbolsStream = null;
			if (_includeSymbols)
			{
				var symbolsPath = Path.Combine(assemblyPath, "..", Path.GetFileNameWithoutExtension(assemblyPath) + ".pdb");
				if (File.Exists(symbolsPath))
				{
					var symbolsBytes = File.ReadAllBytes(symbolsPath);
					symbolsStream = new MemoryStream(symbolsBytes, false);
				}
			}

			var bytes = File.ReadAllBytes(assemblyPath);
			var stream = new MemoryStream(bytes, false);
			return LoadFromStream(stream, symbolsStream);
		}

		return base.LoadFromAssemblyPath(assemblyPath);
	}
}
