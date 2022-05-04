using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Docker.DotNet;


namespace Dotknet.Commands;

public class UploadLayerCommandOptions
{
  public string? Layer { get; set; }
  public string? RegistryPath { get; set; }
}

public interface IUploadLayerCommand
{
  void Execute();
}

public class UploadLayerCommand : IUploadLayerCommand
{
  private readonly UploadLayerCommandOptions _options;
  private readonly ILogger<UploadLayerCommand> _logger;

  public UploadLayerCommand(IOptions<UploadLayerCommandOptions> options, ILogger<UploadLayerCommand> logger)
  {
    _options = options.Value;
    _logger = logger;
  }

  public void Execute()
  {
    _logger.LogInformation("Uploading image at {Layer}", _options.Layer);
    _logger.LogInformation("Uploading image to {RegistryPath}", _options.RegistryPath);


    var client = new DockerClientConfiguration()
     .CreateClient();

  }
}
