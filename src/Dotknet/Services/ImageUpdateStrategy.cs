using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dotknet.Models;
using Dotknet.RegistryClient;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;

namespace Dotknet.Services;

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

  private readonly string _baseImage;
  private readonly string _destinationImage;
  private readonly IManifestRegistryResponse _manifest;

  public SingleManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, string baseImage, string destinationImage, IManifestRegistryResponse manifest)
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
  private readonly string _destinationImage;
  private readonly string _baseImage;
  private readonly IManifestIndex _manifestIndex;
  private readonly IEnumerable<ManifestDescriptor> _manifestDescriptors;

  public MultiManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, string baseImage, string destinationImage, IManifestIndex manifestIndex, IEnumerable<ManifestDescriptor> manifestDescriptors)
  : base(registryClientFactory)
  {
    _destinationImage = destinationImage;
    _baseImage = baseImage;
    _manifestIndex = manifestIndex;
    _manifestDescriptors = manifestDescriptors; 
  }

  public async Task<Hash> UpdateRepositoryImage(ILayer layer)
  {
    var client = _registryClientFactory.Create();
    var layerUploadTask = client.BlobOperations.UploadLayer(_destinationImage, layer);

    var configUpdateTasks = _manifestDescriptors.Select(async md => {
      var config = client.BlobOperations.GetConfig(_baseImage, md.Manifest.Config);

     // Get config object
     // Add layer Diff ID to config object
     // Push config object

     // Add layer digest to manifest object
     // Push Manifest
     // return manifest descriptors
    });



    // 1. Push layer blob
    // 2. Foreach manifest
    //   a. Add layer Diff ID to config object
    //   b. Push config object
    //   c. Add layer Digest to manifest object
    //   d. Add config descriptor to manifest object
    //   e. Push manifest
    // 3. Update manifest index references
    // 4. Push manifest index
    throw new System.NotImplementedException();
  }
}
