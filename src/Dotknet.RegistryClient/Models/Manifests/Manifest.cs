using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dotknet.RegistryClient.Models.Manifests;

public interface IManifestRegistryResponse
{
  int SchemaVersion { get; }
  bool IsManifestIndex { get; }
  MediaTypeEnum MediaType { get; set; }
}

public interface IManifestIndex : IManifestRegistryResponse
{
  IEnumerable<Descriptor> Manifests { get; set; }
  Task<Stream> ToJson();
  public MediaTypeEnum MediaType { get; set; }
}

public interface IManifest : IManifestRegistryResponse
{
  Descriptor Config { get; set; }
  void AddLayer(Descriptor layerDescriptor);
  Task<Descriptor> BuildDescriptor(Descriptor baseDescriptor);
  Task<Stream> ToJson();
  public MediaTypeEnum MediaType { get; set; }
  public IEnumerable<Descriptor> Layers { get; }
}

public abstract class Manifest
{
  public static IManifestRegistryResponse FromContent(string content)
  {
    IManifestRegistryResponse manifest;
    if (IsDockerManifestList(content, out manifest))
    {
      return manifest;
    }
    if (IsDockerManifestSchema2(content, out manifest))
    {
      return manifest;
    }
    throw new NotImplementedException($"Image media type not implemented");
  }

  private static bool IsDockerManifestList(string content, out IManifestRegistryResponse? manifest)
  {
    Func<string, IManifestRegistryResponse> parse = c => DockerManifestList.FromContent(c);
    Func<IManifestRegistryResponse, bool> test = m => ((DockerManifestList)m).MediaType!.Equals(MediaTypeEnum.DockerManifestList);
    return TryParseManifest(content, parse, test, out manifest);
  }

  private static bool IsDockerManifestSchema2(string content, out IManifestRegistryResponse? manifest)
  {
    Func<string, IManifestRegistryResponse> parse = c => DockerManifestSchema2.FromContent(c);
    Func<IManifestRegistryResponse, bool> test = m => ((DockerManifestSchema2)m).MediaType!.Equals(MediaTypeEnum.DockerManifestSchema2);
    return TryParseManifest(content, parse, test, out manifest);
  }

  private static bool TryParseManifest(string content, Func<string, IManifestRegistryResponse> parseManifest, Func<IManifestRegistryResponse, bool> test, out IManifestRegistryResponse? manifest)
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
