using Dotknet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nuke.Common.Tools.DotNet;
using SharpCompress.Archives.Tar;

namespace Dotknet.Commands;

public class PublishCommandOptions
{
  public string? Project { get; set; }
  public string? Output { get; set; }
}

public interface IPublishCommand
{
  void Execute();
}

public class PublishCommand : IPublishCommand
{
  private readonly ILogger<PublishCommand> _logger;
  private readonly PublishCommandOptions _options;

  public PublishCommand(ILogger<PublishCommand> logger, IOptions<PublishCommandOptions> options)
  {
    _logger = logger;
    _options = options.Value;
  }

  private string DotnetBuild() {

    return _options.Output!;
  }

  public void Execute()
  {
    DotNetTasks.DotNetPublish(settings => settings
        .SetProject(_options.Project)
        .SetOutput(_options.Output));

    using var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory(_options.Output!, "dotknet-app");
    using var layer = new TarArchiveLayer(tarArchive);

    _logger.LogInformation("Output will create layer {Layer}", layer);
  }
}
