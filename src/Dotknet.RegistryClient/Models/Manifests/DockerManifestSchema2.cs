using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Extensions;

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

  public void AddLayer(Descriptor layerDescriptor)
  {
    Layers = Layers.Append(layerDescriptor);
  }

  public async Task<Descriptor> BuildDescriptor(Descriptor baseDescriptor)
  {
    using var json = await this.ToJson();

    var descriptor = JsonSerializer.Deserialize<Descriptor>(JsonSerializer.Serialize(baseDescriptor));
    descriptor!.Digest = json.GetHash();
    descriptor!.Size = json.Length;

    return descriptor;
  }

  public async Task<Stream> ToJson()
  {
    var stream = new MemoryStream();
    await JsonSerializer.SerializeAsync(stream, this);
    return stream;
  }
}
