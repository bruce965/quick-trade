using System.CommandLine;

namespace QuickTrade.Options;

class ForceAcceptDefaultsOption : Option<bool>
{
	public static ForceAcceptDefaultsOption Instance { get; } = new();

	ForceAcceptDefaultsOption() : base("--force-yes")
	{
		Description = "Use default options without asking for confirmation, even for destructive operations";
	}
}
