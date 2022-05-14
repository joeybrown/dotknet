using System;

namespace Dotknet.RegistryClient.Models.Manifests;

public interface IImageManifest
{
  int SchemaVersion { get; }
}

public abstract class ImageManifest
{
  public static IImageManifest FromContent(string content)
  {
    if (IsImageManifestList(content, out var manifest)) {
      return manifest;
    }
    throw new NotImplementedException($"Image media type not implemented");
  }

  private static bool IsImageManifestList(string content, out IImageManifest? manifest){
    Func<string, IImageManifest> parse = c => ImageManifestList.FromContent(c);
    Func<IImageManifest, bool> test = m => ((ImageManifestList) m).MediaType!.Equals(MediaTypeEnum.DockerManifestList);
    return TryParseManifest(content, parse, test, out manifest);
  }

  private static bool TryParseManifest(string content, Func<string, IImageManifest> parseManifest, Func<IImageManifest, bool> test, out IImageManifest? manifest)
  {
    try
    {
      manifest = parseManifest(content);
    }
    catch
    {
      manifest = null;
      return false;
    }
    return test(manifest);
  }
}
