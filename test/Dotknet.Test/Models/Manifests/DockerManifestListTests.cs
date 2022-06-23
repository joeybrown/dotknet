using System.IO;
using Dotknet.RegistryClient.Models.Manifests;
using FluentAssertions;
using Xunit;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models.Registry;

public class ImageManifestTests
{
  [Fact]
  public void Create_Manifest_From_Docker_Manifest_List()
  {
    var image = ReadManifestFromFile("DockerManifestList.json");
    image.Should().BeOfType<DockerManifestList>();
  }
}
