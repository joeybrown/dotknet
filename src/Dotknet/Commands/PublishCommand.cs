using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public interface IPublishCommand
{
  void Execute();
}

public class PublishCommand: IPublishCommand
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
    _logger.LogInformation("Project location: {project}", _options.ProjectPath);
  }
}