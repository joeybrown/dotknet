using Dotknet.RegistryClient.Models;
using FluentAssertions;
using Xunit;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models;

public class DescriptorTests
{
  [Fact]
  public void DeserializeDescriptor()
  {
    var descriptor = ReadFromFile<Descriptor>("Descriptor.json");
    descriptor!.MediaType.Should().BeEquivalentTo(MediaTypeEnum.DockerManifestSchema2);
  }
}
