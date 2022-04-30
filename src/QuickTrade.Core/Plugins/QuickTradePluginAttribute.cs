namespace QuickTrade.Core.Plugins;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class QuickTradePluginAttribute : Attribute
{
	/// <summary>
	/// Name of this plugin.
	/// </summary>
	public string? Name { get; }

	public QuickTradePluginAttribute() { }

	public QuickTradePluginAttribute(string? name)
	{
		Name = name;
	}
}
