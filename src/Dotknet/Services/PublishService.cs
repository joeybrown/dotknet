using System.Threading.Tasks;
using Dotknet.Models;
using Dotknet.RegistryClient;
using Dotknet.RegistryClient.Models.Manifests;
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
  private readonly IRegistryClientFactory _registryClientFactory;
  private readonly IArchiveService _archiveService;

  public PublishService(ILogger<PublishService> logger, IDotnetPublishService dotnetPublishService, IRegistryClientFactory registryClientFactory, IArchiveService archiveService)
  {
    _logger = logger;
    _dotnetPublishService = dotnetPublishService;
    _registryClientFactory = registryClientFactory;
    _archiveService = archiveService;
  }

  public async Task Execute(string project, string output, string baseImage, string layerRoot = "dotnet-app")
  {
    // var appLayerTask = BuildLayer(project, output, layerRoot);
    var buildUpdateServiceTask = BuildImageUpdateService(baseImage);
    // await Task.WhenAll(appLayerTask, baseImageManifestTask);
    var service = await buildUpdateServiceTask;
  }

  private async Task<IImageUpdateService> BuildImageUpdateService(string baseImage) {
    var registryClient = _registryClientFactory.Create();
    var manifest = await registryClient.ManifestOperations.GetManifest(baseImage);
    if (!manifest.IsManifestIndex) {
      return new SingleManifestImageUpdateService(baseImage, manifest);
    }
    var manifestIndex = (IManifestIndex) manifest;
    var manifests = await registryClient.ManifestOperations.EnumerateManifests(baseImage, manifestIndex);
    return new MultipleManifestImageUpdateService(baseImage, manifestIndex, manifests);
  }

  private async Task<ILayer> BuildLayer(string project, string output, string layerRoot)
  {
    var dotnetPublishDirectory = await _dotnetPublishService.Execute(project, output);
    var layer = await _archiveService.Execute(dotnetPublishDirectory, layerRoot);
    return (ILayer)layer;
  }
}
