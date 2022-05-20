using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dotknet.RegistryClient.Models;

namespace Dotnet.RegistryClient.Models;

// Ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/config.go
public class ConfigFile
{
  [JsonPropertyName("architecture")]
  public string Architecture { get; set; }

  [JsonPropertyName("author")]
  public string? Author { get; set; }

  [JsonPropertyName("container")]
  public string? Container { get; set; }

  [JsonPropertyName("created")]
  public DateTime? Created { get; set; }

  [JsonPropertyName("docker_version")]
  public string? DockerVersion { get; set; }

  [JsonPropertyName("history")]
  public IEnumerable<History>? History { get; set; }

  [JsonPropertyName("os")]
  public string OS { get; set; }

  [JsonPropertyName("rootfs")]
  public RootFS RootFS { get; set; }

  [JsonPropertyName("config")]
  public Config Config { get; set; }

  [JsonPropertyName("os.version")]
  public string OSVersion { get; set; }
}

public class History
{
  [JsonPropertyName("author")]
  public string? Author { get; set; }

  [JsonPropertyName("created")]
  public DateTime? Created { get; set; }

  [JsonPropertyName("created_by")]
  public string? CreatedBy { get; set; }

  [JsonPropertyName("comment")]
  public string? Comment { get; set; }

  [JsonPropertyName("empty_layer")]
  public bool? EmptyLayer { get; set; }
}

public class RootFS
{

  [JsonPropertyName("type")]
  public string Type { get; set; }

  [JsonPropertyName("diff_ids")]
  public IEnumerable<Hash> DiffIds { get; set; }
}

public class Config
{

}
