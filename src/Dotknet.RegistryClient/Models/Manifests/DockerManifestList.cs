using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dotknet.RegistryClient.Models.Manifests;

public class DockerManifestList : IManifestIndex
{
  [JsonIgnore]
  public bool IsManifestIndex => MediaType.IsManifestIndex;

  [JsonPropertyName("schemaVersion")]
  public int SchemaVersion { get; set; }

  [JsonPropertyName("mediaType")]
  public MediaTypeEnum MediaType { get; set; }

  [JsonPropertyName("annotations")]
  public Dictionary<string, string>? Annotations { get; set; }

  [JsonPropertyName("manifests")]
  public IEnumerable<Descriptor> Manifests { get; set; }

  public static DockerManifestList FromContent(string manifest) =>
    JsonSerializer.Deserialize<DockerManifestList>(manifest)!;

  public async Task<Stream> ToJson()
  {
    var stream = new MemoryStream();
    await JsonSerializer.SerializeAsync(stream, this);
    return stream;
  }
}
