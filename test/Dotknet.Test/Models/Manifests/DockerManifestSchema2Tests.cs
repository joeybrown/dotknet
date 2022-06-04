using Dotknet.RegistryClient.Models.Manifests;
using FluentAssertions;
using Xunit;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models.Registry;

public class DockerManifestSchema2Tests
{
  [Fact]
  public void Create_Manifest_From_Docker_Schema_2()
  {
    var image = ReadManifestFromFile("DockerManifestSchema2.json");
    image.Should().BeOfType<DockerManifestSchema2>();
  }
}
