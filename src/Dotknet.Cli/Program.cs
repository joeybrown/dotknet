namespace Dotknet.Cli;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using static LogEvents;
using Serilog;
using Serilog.Events;

class Program
{
  static async Task Main(string[] args) => await BuildCommandLine()
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
          services.AddSingleton<ILifecycle, Lifecycle>();
        });
        
      })
      .UseDefaults()
      .Build()
      .InvokeAsync(args);

  private static CommandLineBuilder BuildCommandLine()
  {
    var root = new RootCommand();
    var publish = Publish();
    root.Add(publish);
    return new CommandLineBuilder(root);
  }

  private static Command Publish()
  {
    var command = new Command("publish");
    command.AddOption(new Option<DirectoryInfo>("--project-path")
    {
      IsRequired = true
    });
    command.Handler = CommandHandler.Create<LifecycleOptions, IHost>((options, host) =>
    {
      var serviceProvider = host.Services;
      var lifecycle = serviceProvider.GetRequiredService<ILifecycle>();
      var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
      var logger = loggerFactory.CreateLogger(typeof(Program));
      var projectPath = options.ProjectPath;
      logger.LogInformation(PublishEvent, "Publish requested for: {project}", projectPath);
      lifecycle.Publish(options);
    });
    return command;
  }
}
