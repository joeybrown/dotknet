using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nuke.Common.Tools.DotNet;

namespace Dotknet.Services;

public interface IDotnetPublishService
{
  Task<DirectoryInfo> Execute(string project, string output);
}

public class DotnetPublishService : IDotnetPublishService
{
  private readonly ILogger<DotnetPublishService> _logger;

  public DotnetPublishService(ILogger<DotnetPublishService> logger)
  {
    _logger = logger;
  }

  public Task<DirectoryInfo> Execute(string project, string output)
  {
    var outputDirectory = new DirectoryInfo(output);
    if (outputDirectory.Exists)
    {
      outputDirectory.Delete(true);
    }

    var projectDirectory = new DirectoryInfo(project);

    DotNetTasks.DotNetPublish(settings => settings
        .SetProject(projectDirectory.FullName)
        .SetOutput(outputDirectory.FullName));

    return Task.FromResult(outputDirectory);
  }
}
