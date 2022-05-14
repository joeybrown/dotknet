using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient.Models;

// Ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/platform.go
public class PlatformRaw
{
  [JsonPropertyName("architecture")]
  public string Architecture { get; set; }
  [JsonPropertyName("os")]
  public string OS { get; set; }
  [JsonPropertyName("os.version")]
  public string? OSVersion { get; set; }
  [JsonPropertyName("os.features")]
  public IEnumerable<string>? OSFeatures { get; set; }
  [JsonPropertyName("variant")]
  public string? Variant { get; set; }
  [JsonPropertyName("features")]
  public IEnumerable<string>? Features { get; set; }
}
