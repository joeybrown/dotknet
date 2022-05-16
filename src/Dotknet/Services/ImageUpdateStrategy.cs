using System.Collections.Generic;
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

  private readonly string _image;
  private readonly IManifest _manifest;

  public SingleManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, string image, IManifest manifest)
  : base(registryClientFactory)
  {
    _image = image;
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
  private string _image;
  private IManifestIndex _manifestIndex;
  private IEnumerable<IManifest> _manifests;

  public MultiManifestRepositoryUpdateStrategy(IRegistryClientFactory registryClientFactory, string image, IManifestIndex manifestIndex, IEnumerable<IManifest> manifests)
  : base(registryClientFactory)
  {
    _image = image;
    _manifestIndex = manifestIndex;
    _manifests = manifests;
  }

  public async Task<Hash> UpdateRepositoryImage(ILayer layer)
  {
    var client = _registryClientFactory.Create();
    var response = await client.BlobOperations.UploadBlob("http://localhost:5000", "foo", layer);

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
