using System.Collections.Generic;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models.Manifests;

namespace Dotknet.Services;

public interface IImageUpdateService {
  Task UpdateRepositoryImage();
}

public class SingleManifestImageUpdateService: IImageUpdateService {

  private readonly string _image;
  private readonly IManifest _manifest;

  public SingleManifestImageUpdateService(string image, IManifest manifest)
  {
    _image = image;
    _manifest = manifest;
  }

  public Task UpdateRepositoryImage()
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

public class MultipleManifestImageUpdateService: IImageUpdateService {
  private string _image;
  private IManifestIndex _manifestIndex;
  private IEnumerable<IManifest> _manifests;

  public MultipleManifestImageUpdateService(string image, IManifestIndex manifestIndex, IEnumerable<IManifest> manifests)
  {
    _image = image;
    _manifestIndex = manifestIndex;
    _manifests = manifests;
  }

  public Task UpdateRepositoryImage()
  {
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
