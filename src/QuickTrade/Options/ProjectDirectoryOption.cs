// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.CommandLine;

namespace QuickTrade.Options;

class ProjectDirectoryOption : Option<DirectoryInfo>
{
	public static ProjectDirectoryOption Instance { get; } = new();

	public static DirectoryInfo DefaultValue = new(".");

	ProjectDirectoryOption() : base("--project")
	{
		Description = Strings.Option_ProjectDirectory;

		AddAlias("-p");

		SetDefaultValueFactory(() => DefaultValue);
	}
}
