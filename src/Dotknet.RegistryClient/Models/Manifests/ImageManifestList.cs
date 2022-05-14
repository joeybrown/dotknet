using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient.Models.Manifests;

public class ImageManifestList : IImageManifest
{
  [JsonPropertyName("schemaVersion")]
  public int SchemaVersion {get;set;}

  [JsonPropertyName("mediaType")]
  public string? MediaType {get;set;}

  [JsonPropertyName("annotations")]
  public Dictionary<string, string>? Annotations {get;set;}

  [JsonPropertyName("manifests")]
  public IEnumerable<DescriptorRaw> Manifests {get;set;}

  public static ImageManifestList FromContent(string manifest){
    return JsonSerializer.Deserialize<ImageManifestList>(manifest)!;
  }
}
