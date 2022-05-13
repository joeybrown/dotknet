using System;

namespace Dotknet.RegistryClient.Models.Manifests;

public interface IImageManifest
{
  int SchemaVersion { get; }
}

public abstract class ImageManifest
{
  public static IImageManifest FromContent(string manifest)
  {
    var mediaType = GetMediaType(manifest);
    switch (mediaType)
    {
      case MediaType.DockerManifestSchema2:
        return ImageManifestDocker2_2.FromContent(manifest);
      default:
        throw new NotImplementedException($"Image media type not implemented");
    }
  }

  private static MediaType GetMediaType(string manifest)
  {
    return MediaType.DockerManifestSchema2;
  }
}
