namespace Dotknet.Cli;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using Serilog;
using Serilog.Events;
using Dotknet.Services;
using Dotknet.Cli.Options;
using Microsoft.Extensions.Options;
using Dotknet.RegistryClient;

class Program
{
  static async Task Main(string[] args) =>
    await BuildCommandLine()
      .UseHost(_ => Host.CreateDefaultBuilder(), host =>
      {
        host.ConfigureLogging((context, config) =>
        {
          config.ClearProviders();
          Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Dotknet", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
          config.AddSerilog(Log.Logger);
        });

        host.ConfigureServices(services =>
        {
          services.AddOptions<PublishCommandOptions>().BindCommandLine();
          services.AddSingleton<IPublishService, PublishService>();
          services.AddSingleton<IDotnetPublishService, DotnetPublishService>();
          services.AddSingleton<IArchiveService, ArchiveService>();
          services.AddRegistryClientServices(x=>{});
        });
      })
      .UseDefaults()
      .Build()
      .InvokeAsync(args);

  private static CommandLineBuilder BuildCommandLine()
  {
    var root = new RootCommand();
    root.Add(Publish);
    return new CommandLineBuilder(root);
  }

  private static Command Publish
  {
    get
    {
      var command = new Command("publish");
      command.AddOption(new Option<string>("--project")
      {
        IsRequired = true,
      });
      command.AddOption(new Option<string>("--output")
      {
        IsRequired = true,
      });
      command.AddOption(new Option<string>("--baseImage")
      {
        IsRequired = true,
      });
      command.Handler = CommandHandler.Create<IHost>(async host => {
        var options = host.Services.GetRequiredService<IOptions<PublishCommandOptions>>().Value;
        await host.Services.GetRequiredService<IPublishService>().Execute(options.Project!, options.Output!, options.BaseImage!);
      });
      return command;
    }
  }
}
