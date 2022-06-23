using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Extensions;
using Dotknet.RegistryClient.Models;

namespace Dotnet.RegistryClient.Models;

// Ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/config.go
public class ConfigFile
{
  [JsonPropertyName("architecture")]
  public string Architecture { get; set; }

  [JsonPropertyName("author")]
  public string? Author { get; set; }

  public ConfigFile SetWorkingDir(string workingDir)
  {
    Config.SetWorkingDir(workingDir);
    return this;
  }

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

  public ConfigFile AddLayer(Hash diffId)
  {
    RootFS.AddLayer(diffId);
    return this;
  }


  public ConfigFile SetEntrypoint(params string[] arguments)
  {
    Config.SetEntrypoint(arguments);
    return this;
  }

  public async Task<Descriptor> BuildDescriptor(Descriptor baseDescriptor)
  {
    var descriptor = JsonSerializer.Deserialize<Descriptor>(JsonSerializer.Serialize(baseDescriptor));

    using var stream = new MemoryStream();
    await JsonSerializer.SerializeAsync(stream, this);

    descriptor!.Digest = stream.GetHash();
    descriptor!.Size = stream.Length;

    return descriptor;
  }
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

  internal RootFS AddLayer(Hash diffId)
  {
    DiffIds = DiffIds.Append(diffId);
    return this;
  }
}

public class HealthConfig
{
  [JsonPropertyName("Test")]
  public IEnumerable<string>? Test { get; set; }

  [JsonPropertyName("Interval")]
  public TimeSpan? Interval { get; set; }

  [JsonPropertyName("Timeout")]
  public TimeSpan? Timeout { get; set; }

  [JsonPropertyName("StartPeriod")]
  public TimeSpan? StartPeriod { get; set; }

  [JsonPropertyName("Retries")]
  public int? Retries { get; set; }
}

public class Config
{
  [JsonPropertyName("AttachStderr")]
  public bool? AttachStderr { get; set; }

  [JsonPropertyName("AttachStdin")]
  public bool? AttachStdin { get; set; }

  [JsonPropertyName("AttachStdout")]
  public bool? AttachStdout { get; set; }

  [JsonPropertyName("Cmd")]
  public IEnumerable<string>? Cmd { get; set; }

  [JsonPropertyName("Healthcheck")]
  public HealthConfig? Healthcheck { get; set; }

  [JsonPropertyName("Domainname")]
  public string? Domainname { get; set; }

  [JsonPropertyName("Entrypoint")]
  public IEnumerable<string>? Entrypoint { get; set; }

  [JsonPropertyName("Env")]
  public IEnumerable<string>? Env { get; set; }

  [JsonPropertyName("Hostname")]
  public string? Hostname { get; set; }

  [JsonPropertyName("Image")]
  public string? Image { get; set; }

  [JsonPropertyName("Labels")]
  public Dictionary<string, string>? Labels { get; set; }

  [JsonPropertyName("OnBuild")]
  public IEnumerable<string>? OnBuild { get; set; }

  [JsonPropertyName("OpenStdin")]
  public bool? OpenStdin { get; set; }

  [JsonPropertyName("StdinOnce")]
  public bool? StdinOnce { get; set; }

  [JsonPropertyName("Tty")]
  public bool? Tty { get; set; }

  [JsonPropertyName("User")]
  public string? User { get; set; }

  [JsonPropertyName("Volumes")]
  public Dictionary<string, object>? Volumes { get; set; }

  [JsonPropertyName("WorkingDir")]
  public string? WorkingDir { get; set; }

  [JsonPropertyName("ExposedPorts")]
  public Dictionary<string, object>? ExposedPorts { get; set; }

  [JsonPropertyName("ArgsEscaped")]
  public bool? ArgsEscaped { get; set; }

  [JsonPropertyName("NetworkDisabled")]
  public bool? NetworkDisabled { get; set; }

  [JsonPropertyName("MacAddress")]
  public string? MacAddress { get; set; }

  [JsonPropertyName("StopSignal")]
  public string? StopSignal { get; set; }

  [JsonPropertyName("Shell")]
  public IEnumerable<string>? Shell { get; set; }

  internal Config SetEntrypoint(params string[] arguments)
  {
    Entrypoint = arguments;
    return this;
  }

  internal Config SetWorkingDir(string workingDir)
  {
    WorkingDir = workingDir;
    return this;
  }
}
