using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotknet.RegistryClient;

/// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/hash.go
[JsonConverter(typeof(HashConverter))] 
public class Hash
{
  // Algorithm holds the algorithm used to compute the hash.
  public string? Algorithm { get; set; }

  // Hex holds the hex portion of the content hash.
  public string? Hex { get; set; }

  public override string ToString()
  {
    return $"{Algorithm}:{Hex}";
  }
}

public class HashConverter : JsonConverter<Hash>
{
  public override Hash? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var hash = reader.GetString();
    if (string.IsNullOrWhiteSpace(hash)){
      return null;
    }
    var parts = hash.Split(":");
    return new Hash{
      Algorithm = parts[0],
      Hex = parts[1]
    };
  }

  public override void Write(Utf8JsonWriter writer, Hash value, JsonSerializerOptions options)
  {
    writer.WriteRawValue(value.ToString());
  }
}