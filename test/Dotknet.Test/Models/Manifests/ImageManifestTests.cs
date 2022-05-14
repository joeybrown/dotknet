using System.IO;
using Dotknet.RegistryClient.Models.Manifests;
using FluentAssertions;
using Xunit;

namespace Dotknet.Test.Models.Registry;

public class ImageManifestTests {
  [Fact]
  public void Create_Manifest_From_Docker_Manifest_List() {
    var sourceFile = Path.Join("TestResources", "ImageManifestListBusybox.json");
    string content = File.ReadAllText(sourceFile);
    var image = ImageManifest.FromContent(content);
    image.Should().BeOfType<ImageManifestList>();
  }
}
