// Copyright (c) 2022 Fabio Iotti
// The copyright holders license this file to you under the MIT license,
// available at https://github.com/bruce965/quick-trade/raw/master/LICENSE

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuickTrade.Options;
using System.CommandLine;

namespace QuickTrade.Commands;

class StartCommand : Command
{
	public static StartCommand Instance { get; } = new();

	StartCommand() : base("start")
	{
		Description = "Start service";

		AddCommand(NewProjectCommand.Instance);

		this.SetHandler(
			(Func<DirectoryInfo, Task>)Execute,
			ProjectDirectoryOption.Instance);
	}

	static Task Execute(DirectoryInfo projectDirectory)
	{
		var builder = CreateHostBuilder(projectDirectory.FullName);

		builder.ConfigureServices(services =>
		{
			//services.
		});

		return builder.Build().RunAsync();
	}

	static IHostBuilder CreateHostBuilder(string projectDirectory)
	{
		var builder = new HostBuilder();

		builder.UseContentRoot(projectDirectory);

		builder.ConfigureAppConfiguration((hostingContext, config) =>
		{
			config.AddJsonFile(projectDirectory, optional: false, reloadOnChange: true);
		});

		builder.ConfigureLogging((hostingContext, logging) =>
		{
			logging.AddConsole();

			logging.Configure(options =>
			{
				options.ActivityTrackingOptions =
					ActivityTrackingOptions.SpanId |
					ActivityTrackingOptions.TraceId |
					ActivityTrackingOptions.ParentId;
			});
		});

		builder.UseDefaultServiceProvider((context, options) =>
		{
			#if DEBUG

			options.ValidateScopes = true;
			options.ValidateOnBuild = true;

			#endif
		});

		return builder;
	}
}
