using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient.Models.Manifests;

public class DockerManifestList : IManifestIndex
{
  [JsonIgnore]
  public bool IsManifestIndex => MediaType.IsManifestIndex;

  [JsonPropertyName("schemaVersion")]
  public int SchemaVersion {get;set;}

  [JsonPropertyName("mediaType")]
  public MediaTypeEnum MediaType {get;set;}

  [JsonPropertyName("annotations")]
  public Dictionary<string, string>? Annotations {get;set;}

  [JsonPropertyName("manifests")]
  public IEnumerable<Descriptor> Manifests {get;set;}

  public static DockerManifestList FromContent(string manifest) => 
    JsonSerializer.Deserialize<DockerManifestList>(manifest)!;
}
