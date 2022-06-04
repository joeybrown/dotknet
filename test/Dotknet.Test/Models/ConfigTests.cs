using Dotnet.RegistryClient.Models;
using FluentAssertions;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models;

public class ConfigTests {
  public void SetEntrypoint_Should_Set_Expected() {
    var config = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    config.SetEntrypoint("dotnet", "foo.dll"); 
    config.Config.Entrypoint.Should().BeEquivalentTo("dotnet", "foo.dll");
  }
}