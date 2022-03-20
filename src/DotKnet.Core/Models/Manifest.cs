namespace DotKnet.Core.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

public class MediaType
{
  public const string Layer = "application/vnd.docker.image.rootfs.diff.tar.gzip";
  public const string ContainerConfigJson = "application/vnd.docker.container.image.v1+json";
}

public class Hash
{
  private Hash(string algorithm, string hex)
  {
    Algorithm = algorithm;
    Hex = hex;
  }

  public static Hash FromFullyQualified(string value)
  {
    var parts = value.Split(":");
    return new Hash(parts[0], parts[1]);
  }

  public string Algorithm { get; private set; }
  public string Hex { get; private set; }
}

public interface IDescriptor {
  string MediaType {get;}
  int Size {get;}
  Hash Digest {get;}
}

public class LayerDescriptor: IDescriptor
{
  public string MediaType => Models.MediaType.Layer;
  public LayerDescriptor(int size, string digest)
  {
    Size = size;
    Digest = Hash.FromFullyQualified(digest);
  }

  public int Size { get; }
  public Hash Digest { get; }
}

public class ConfigDescriptor: IDescriptor
{
  public string MediaType => Models.MediaType.ContainerConfigJson;
  public ConfigDescriptor(int size, string digest)
  {
    Size = size;
    Digest = Hash.FromFullyQualified(digest);
  }

  public int Size { get; }
  public Hash Digest { get; }
}

public class Manifest
{
  private class DescriptorDto
  {
    [JsonPropertyName("mediaType")]
    public string MediaType { get; set; }
    [JsonPropertyName("size")]
    public int Size { get; set; }
    [JsonPropertyName("digest")]
    public string Digest { get; set; }
  }

  private class ManifestDto
  {
    [JsonPropertyName("layers")]

    public IEnumerable<DescriptorDto> Layers { get; set; } = Array.Empty<DescriptorDto>();
    [JsonPropertyName("config")]
    public DescriptorDto Config {get;set;}
  }

  public IEnumerable<LayerDescriptor> Layers { get; private set; }
  public ConfigDescriptor Config {get; private set;}

  private Manifest(ConfigDescriptor config, IEnumerable<LayerDescriptor> layers)
  {
    Layers = layers;
    Config = config;
  }

  public Manifest AddLayer(LayerDescriptor layer)
  {
    Layers = Layers.Append(layer);
    return this;
  }

  public static Manifest FromJson(string json)
  {
    var manifestDto = JsonSerializer.Deserialize<ManifestDto>(json);
    var layers = manifestDto!.Layers.Select(x => new LayerDescriptor(x.Size, x.Digest));
    var config = new ConfigDescriptor(manifestDto!.Config.Size, manifestDto!.Config.Digest);
    return new Manifest(config, layers);
  }
}
