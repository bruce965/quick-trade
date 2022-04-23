using System.CommandLine;

namespace QuickTrade.Options;

class AcceptDefaultsOption : Option<bool>
{
	public static AcceptDefaultsOption Instance { get; } = new();

	AcceptDefaultsOption() : base("--yes")
	{
		Description = "Use default options without asking for confirmation";

		AddAlias("-y");
	}
}
