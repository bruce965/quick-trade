// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using System.Text;
using System.Text.Json;

namespace QuickTrade.Configuration;

class ProjectHelper
{
	static readonly JsonSerializerOptions _options = new()
	{
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
		WriteIndented = true,
	};

	static readonly Encoding _encoding = new UTF8Encoding(false);

	public static string FileName => "quicktrade.jsonc";

	public static int LatestVersion => 1;

	public static string DefaultPluginsDirName => "plugins";

	public static async ValueTask<QuickTradeProject> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
	{
		var configuration = await JsonSerializer.DeserializeAsync<QuickTradeProject>(stream, _options, cancellationToken);

		return configuration ?? new QuickTradeProject();
	}

	public static async ValueTask SaveAsync(Stream stream, QuickTradeProject configuration, CancellationToken cancellationToken = default)
	{
		using (var writer = new StreamWriter(stream, _encoding, leaveOpen: true))
		{
			// TODO: cancellationToken (currently not supported).
			await writer.WriteLineAsync("/* QuickTrade Project */");

			await writer.FlushAsync();
		}

		await JsonSerializer.SerializeAsync(stream, configuration, _options, cancellationToken);
	}

	public static async ValueTask<QuickTradeProject?> LoadFromProjectDirectoryAsync(DirectoryInfo projectDirectory, CancellationToken cancellationToken = default)
	{
		var configFile = new FileInfo(Path.Combine(projectDirectory.FullName, FileName));
		if (!configFile.Exists)
		{
			Console.Error.WriteLine(Strings.Error_InvalidProjectDirectory, FileName);
			return null;
		}

		try
		{
			using var stream = configFile.OpenRead();
			return await LoadAsync(stream, cancellationToken);
		}
		catch (JsonException e)
		{
			Console.Error.WriteLine(Strings.Error_InvalidProjectFile, configFile.FullName, e.Message);
			return null;
		}
	}
}
