using System.CommandLine;

namespace QuickTrade.Options;

class ProjectDirectoryOption : Option<DirectoryInfo>
{
	public static ProjectDirectoryOption Instance { get; } = new();

	public static DirectoryInfo DefaultValue = new DirectoryInfo(".");

	ProjectDirectoryOption() : base("--project")
	{
		Description = "Project directory";

		AddAlias("-p");

		SetDefaultValueFactory(() => DefaultValue);
	}
}
