using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuke.Common.Tools.DotNet;

namespace Dotknet.Commands;

public interface IPublishCommand
{
  void Execute();
}

public class PublishCommand : IPublishCommand
{
  private readonly ILogger<PublishCommand> _logger;
  private readonly LifecycleOptions _options;

  public PublishCommand(ILogger<PublishCommand> logger, IOptions<LifecycleOptions> options)
  {
    _logger = logger;
    _options = options.Value;
  }

  public void Execute()
  {
    var output = DotNetTasks.DotNetPublish(settings =>
    {
      settings.SetProject(_options.Project);
      settings.SetOutput(_options.Output);
      return settings;
    });

    

    _logger.LogInformation("Project: {project}; Output: {output}", _options.Project, _options.Output);
  }
}