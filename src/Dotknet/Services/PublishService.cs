using System.Threading.Tasks;
using Dotknet.RegistryClient;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;
using Microsoft.Extensions.Logging;

namespace Dotknet.Services;

public interface IPublishService
{
  Task Execute(string project, string output, IImageReference baseImage, IImageReference destinationImage, bool skipDotnetBuild, string layerRoot);
}

public class PublishService : IPublishService
{
  private readonly ILogger<PublishService> _logger;
  private readonly IDotnetPublishService _dotnetPublishService;
  private readonly IRegistryClientFactory _registryClientFactory;
  private readonly IArchiveService _archiveService;

  public PublishService(
    ILogger<PublishService> logger, 
    IDotnetPublishService dotnetPublishService, 
    IRegistryClientFactory registryClientFactory, 
    IArchiveService archiveService)
  {
    _logger = logger;
    _dotnetPublishService = dotnetPublishService;
    _registryClientFactory = registryClientFactory;
    _archiveService = archiveService;
  }

  public async Task Execute(string project, string output, IImageReference baseImage, IImageReference destinationImage, bool skipDotnetBuild, string layerRoot)
  {
    var buildLayerTask = BuildLayer(project, output, skipDotnetBuild, layerRoot);
    var buildUpdateServiceTask = BuildImageUpdateService(baseImage, destinationImage);
    
    await Task.WhenAll(buildLayerTask, buildUpdateServiceTask);

    var layer = buildLayerTask.Result;
    var updateService = buildUpdateServiceTask.Result;

    var hash = await updateService.UpdateRepositoryImage(layer);

    _logger.LogInformation("Image loaded. Digest: {Digest}", hash.ToString());

    layer.Dispose();
  }

  private async Task<ImageUpdateStrategy> BuildImageUpdateService(IImageReference baseImage, IImageReference destinationImage) {
    var registryClient = _registryClientFactory.Create();
    var manifest = await registryClient.ManifestOperations.GetManifest(baseImage);
    if (!manifest.IsManifestIndex) {
      return new SingleManifestRepositoryUpdateStrategy(_registryClientFactory, baseImage, destinationImage, manifest);
    }
    var manifestIndex = (IManifestIndex) manifest;
    var manifests = await registryClient.ManifestOperations.EnumerateManifests(baseImage, manifestIndex);
    return new MultiManifestRepositoryUpdateStrategy(_registryClientFactory, baseImage, destinationImage, manifestIndex, manifests);
  }

  private async Task<ILayer> BuildLayer(string project, string output, bool skipDotnetBuild, string layerRoot)
  {
    if (!skipDotnetBuild) {
      await _dotnetPublishService.Execute(project, output);
    }
    var layer = await _archiveService.Execute(output, layerRoot);
    return (ILayer)layer;
  }
}
