using System;
using System.Collections.Generic;

namespace Dotknet.RegistryClient.Models.Manifests;

public interface IManifest
{
  int SchemaVersion { get; }
  bool IsManifestIndex {get;}
}

public interface IManifestIndex: IManifest {
  IEnumerable<Descriptor> Manifests { get; }
}

public abstract class Manifest
{
  public static IManifest FromContent(string content)
  {
    IManifest manifest;
    if (IsDockerManifestList(content, out manifest)) {
      return manifest;
    }
    if (IsDockerManifestSchema2(content, out manifest)) {
      return manifest;
    }
    throw new NotImplementedException($"Image media type not implemented");
  }

  private static bool IsDockerManifestList(string content, out IManifest? manifest){
    Func<string, IManifest> parse = c => DockerManifestList.FromContent(c);
    Func<IManifest, bool> test = m => ((DockerManifestList) m).MediaType!.Equals(MediaTypeEnum.DockerManifestList);
    return TryParseManifest(content, parse, test, out manifest);
  }

  private static bool IsDockerManifestSchema2(string content, out IManifest? manifest){
    Func<string, IManifest> parse = c => DockerManifestSchema2.FromContent(c);
    Func<IManifest, bool> test = m => ((DockerManifestSchema2) m).MediaType!.Equals(MediaTypeEnum.DockerManifestSchema2);
    return TryParseManifest(content, parse, test, out manifest);
  }

  private static bool TryParseManifest(string content, Func<string, IManifest> parseManifest, Func<IManifest, bool> test, out IManifest? manifest)
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
