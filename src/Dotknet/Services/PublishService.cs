using System.Threading.Tasks;
using Dotknet.Clients;
using Dotknet.Models;
using Microsoft.Extensions.Logging;

namespace Dotknet.Services;

public interface IPublishService
{
  Task Execute(string project, string output, string baseImage, string layerRoot = "dotnet-app");
}

public class PublishService : IPublishService
{
  private readonly ILogger<PublishService> _logger;
  private readonly IDotnetPublishService _dotnetPublishService;
  private readonly IRegistryClient _registryClient;
  private readonly IArchiveService _archiveService;

  public PublishService(ILogger<PublishService> logger, IDotnetPublishService dotnetPublishService, IRegistryClient registryClient, IArchiveService archiveService)
  {
    _logger = logger;
    _dotnetPublishService = dotnetPublishService;
    _registryClient = registryClient;
    _archiveService = archiveService;
  }

  public async Task Execute(string project, string output, string baseImage, string layerRoot = "dotnet-app")
  {
    var appLayerTask = BuildLayer(project, output, layerRoot);
    var baseImageManifestTask = _registryClient.GetManifest(baseImage);
    await Task.WhenAll(appLayerTask, baseImageManifestTask);
  }

  private async Task<ILayer> BuildLayer(string project, string output, string layerRoot)
  {
    var dotnetPublishDirectory = await _dotnetPublishService.Execute(project, output);
    var layer = await _archiveService.Execute(dotnetPublishDirectory, layerRoot);
    return (ILayer)layer;
  }
}
