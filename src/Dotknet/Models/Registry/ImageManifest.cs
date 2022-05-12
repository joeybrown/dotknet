using System;

namespace Dotknet.Models.Registry;

public interface IImageManifest {
  int SchemaVersion {get;}
}

public class ImageManifestDocker2_2 : IImageManifest
{
  public int SchemaVersion {get;set;}

  public static ImageManifestDocker2_2 FromContent(string manifest){
    return new ImageManifestDocker2_2();
  }
}

public abstract class ImageManifest {
  public static IImageManifest FromContent(string manifest) {
    var mediaType = GetMediaType(manifest);
    switch (mediaType)
    {
      case MediaType.DockerManifestSchema2:
        return ImageManifestDocker2_2.FromContent(manifest);
      default:
        throw new NotImplementedException($"Image media type not implemented");
    }
  }

  private static MediaType GetMediaType(string manifest) {
    return MediaType.DockerManifestSchema2;
  }
}
