using System.IO;
using Dotknet.RegistryClient.Models.Manifests;
using FluentAssertions;
using Xunit;

namespace Dotknet.Test.Models.Registry;

public class DockerManifestSchema2Tests
{
  [Fact]
  public void Create_Manifest_From_Docker_Schema_2()
  {
    var sourceFile = Path.Join("TestResources", "DockerManifestSchema2.json");
    string content = File.ReadAllText(sourceFile);
    var image = Manifest.FromContent(content);
    image.Should().BeOfType<DockerManifestSchema2>();
  }
}
