using System.IO;
using System.Text.Json;
using Dotknet.RegistryClient.Models;
using FluentAssertions;
using Xunit;

namespace Dotknet.Test.Models;

public class DescriptorTests{
  [Fact]
  public void DeserializeDescriptor() {
    var sourceFile = Path.Join("TestResources", "Descriptor.json");
    string content = File.ReadAllText(sourceFile);
    var deserialized = JsonSerializer.Deserialize<Descriptor>(content);
    deserialized!.MediaType.Should().BeEquivalentTo(MediaTypeEnum.DockerManifestSchema2);
  }
}
