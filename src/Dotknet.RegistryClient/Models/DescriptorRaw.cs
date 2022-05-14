using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient.Models;

// Ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/manifest.go
public class DescriptorRaw
{
  [JsonPropertyName("mediaType")]
  public string MediaType { get; set; }
  [JsonPropertyName("size")]
  public long Size { get; set; }
  [JsonPropertyName("digest")]
  public string Digest { get; set; }
  [JsonPropertyName("data")]
  public byte[]? Data { get; set; }
  [JsonPropertyName("urls")]
  public IEnumerable<string>? Urls { get; set; }
  [JsonPropertyName("annotations")]
  public Dictionary<string, string>? Annotations { get; set; }
  [JsonPropertyName("platform")]
  public PlatformRaw? Platform { get; set; }
}
