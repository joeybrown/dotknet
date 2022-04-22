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
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
          config.AddSerilog(Log.Logger);
        });

        host.ConfigureServices(services =>
        {
          services.AddOptions<LifecycleOptions>().BindCommandLine();
          services.AddSingleton<IPublishCommand, PublishCommand>();
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
      command.AddOption(new Option<DirectoryInfo>("--project-path")
      {
        IsRequired = true,
      });
      command.Handler = CommandHandler.Create<IHost>(host => host.Services.GetRequiredService<IPublishCommand>().Execute());
      return command;
    }
  }
}
