using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient.Models.Manifests;

public class DockerManifestSchema2 : IManifest
{
  [JsonIgnore]
  public bool IsManifestIndex => MediaType.IsManifestIndex;

  [JsonPropertyName("mediaType")]
  public MediaTypeEnum MediaType { get; set; }

  [JsonPropertyName("schemaVersion")]
  public int SchemaVersion { get; set; }

  [JsonPropertyName("config")]
  public Descriptor Config { get; set; }

  [JsonPropertyName("layers")]
  public IEnumerable<Descriptor> Layers { get; set; }

  public static DockerManifestSchema2 FromContent(string manifest) =>
    JsonSerializer.Deserialize<DockerManifestSchema2>(manifest)!;
}
