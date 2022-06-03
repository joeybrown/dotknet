using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dotknet.RegistryClient;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;
using Microsoft.Extensions.Logging;

namespace Dotknet;

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

public interface ImageUpdateStrategy
{
  Task<Hash> UpdateRepositoryImage(ILayer layer);
}

public abstract class AbstractManifestRepositoryUpdateStrategy
{
  public readonly IRegistryClientFactory _registryClientFactory;

  public AbstractManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory)
  {
    _registryClientFactory = registryClientFactory;
  }

}

public class SingleManifestRepositoryUpdateStrategy : AbstractManifestRepositoryUpdateStrategy, ImageUpdateStrategy
{

  private readonly IImageReference _baseImage;
  private readonly IImageReference _destinationImage;
  private readonly IManifestRegistryResponse _manifest;

  public SingleManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, IImageReference baseImage, IImageReference destinationImage, IManifestRegistryResponse manifest)
  : base(registryClientFactory)
  {
    _baseImage = baseImage;
    _destinationImage = destinationImage;
    _manifest = manifest;
  }

  public Task<Hash> UpdateRepositoryImage(ILayer layer)
  {
    // 1. Push layer blob
    // 2. Add layer Diff ID to config object
    // 3. Push config object
    // 4. Add layer Digest to manifest object
    // 5. Add config descriptor to manifest object
    // 6. Push manifest
    throw new System.NotImplementedException();
  }
}

public class MultiManifestRepositoryUpdateStrategy : AbstractManifestRepositoryUpdateStrategy, ImageUpdateStrategy
{
  private readonly IImageReference _destinationImage;
  private readonly IImageReference _baseImage;
  private readonly IManifestIndex _manifestIndex;
  private readonly IEnumerable<ManifestDescriptor> _manifestDescriptors;

  public MultiManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, IImageReference baseImage, IImageReference destinationImage, IManifestIndex manifestIndex, IEnumerable<ManifestDescriptor> manifestDescriptors)
  : base(registryClientFactory)
  {
    _destinationImage = destinationImage;
    _baseImage = baseImage;
    _manifestIndex = manifestIndex;
    _manifestDescriptors = manifestDescriptors.Where(md =>
      (md.Descriptor.Platform?.OS.Equals("linux", System.StringComparison.InvariantCultureIgnoreCase) ?? false) &&
      (md.Descriptor.Platform?.Architecture.Equals("amd64", System.StringComparison.InvariantCultureIgnoreCase) ?? false));
  }

  public async Task<Hash> UpdateRepositoryImage(ILayer layer)
  {
    var client = _registryClientFactory.Create();
    var (layerDescriptor, diffId) = await client.BlobOperations.UploadLayer(_destinationImage, layer);

    var digests = _manifestDescriptors.SelectMany(md => md.Manifest.Layers).Select(x => x.Digest).Distinct();

    foreach (var digest in digests)
    {
      await client.BlobOperations.CopyLayer(_baseImage, _destinationImage, digest);
    }

    var manifestsUpdateTasks = _manifestDescriptors.Select(async md =>
    {
      // Update Manifest Config File
      var config = await client.BlobOperations.GetConfig(_baseImage, md.Manifest.Config);
      config.AddLayer(diffId);
      var configDescriptor = await client.BlobOperations.UploadConfig(_destinationImage, config, md.Manifest.Config);

      // Update Manifest
      md.Manifest.Config = configDescriptor;
      md.Manifest.AddLayer(layerDescriptor);
      return await client.ManifestOperations.UploadManifest(_destinationImage, md.Manifest, md.Descriptor);
    }).ToArray();

    await Task.WhenAll(manifestsUpdateTasks);

    _manifestIndex.Manifests = manifestsUpdateTasks.Select(x => x.Result);

    return await client.ManifestOperations.UploadManifest(_destinationImage, _manifestIndex);
  }
}

