using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Dotknet.Commands;

public class UploadLayerCommandOptions
{
  public string? Layer { get; set; }
  public string? RegistryPath { get; set; }
}

public interface IUploadLayerCommand
{
  Task Execute();
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

  public Task Execute()
  {
    _logger.LogInformation("Uploading image at {Layer}", _options.Layer);
    _logger.LogInformation("Uploading image to {RegistryPath}", _options.RegistryPath);

    if (_options.RegistryPath!.StartsWith("dotknet://"))
    {
      // local daemon
      using var client = new DockerClientConfiguration().CreateClient();
      var imageCreateParameter = new ImagesCreateParameters();
      using Stream imageStream = new MemoryStream();
      var authConfig = new AuthConfig();
      var progress = new Progress<JSONMessage>(m => _logger.LogInformation("{@ImageProgress}", m));
      client.Images.CreateImageAsync(imageCreateParameter, imageStream, authConfig, progress);
    }
    else
    {
      _logger.LogWarning("Only docker daemon is supported at this time.");
    }
    return Task.CompletedTask;
  }
}
