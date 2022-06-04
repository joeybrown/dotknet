using System.IO;
using System.Text.Json;
using Dotknet.RegistryClient.Models.Manifests;

namespace Dotknet.Test.TestResources;

public static class TestResourceHelpers
{
  public static T ReadFromFile<T>(string filename)
  {
    var sourceFile = Path.Join("TestResources", filename);
    string content = File.ReadAllText(sourceFile);
    var deserialized = JsonSerializer.Deserialize<T>(content);
    return deserialized;
  }

  public static IManifestRegistryResponse ReadManifestFromFile(string filename)
  {
    var sourceFile = Path.Join("TestResources", filename);
    string content = File.ReadAllText(sourceFile);
    var image = Manifest.FromContent(content);
    return image;
  }
}