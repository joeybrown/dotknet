using Microsoft.Extensions.Logging;

public class LifecycleOptions
{
  public DirectoryInfo? ProjectPath { get; set; }
}

public interface ILifecycle
{
  void Publish(LifecycleOptions options);
}

public class Lifecycle : ILifecycle
{
  private readonly ILogger<Lifecycle> _logger;

  public Lifecycle(ILogger<Lifecycle> logger)
  {
    _logger = logger;
  }
  public void Publish(LifecycleOptions options)
  {
    _logger.LogInformation("Project location: {project}", options.ProjectPath);
  }
}