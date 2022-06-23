using Dotnet.RegistryClient.Models;
using FluentAssertions;
using Xunit;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models;

public class ConfigTests {

  [Fact]
  public void SetEntrypoint_Should_Set_Expected() {
    var sut = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    sut.SetEntrypoint("dotnet", "foo.dll"); 
    sut.Config.Entrypoint.Should().BeEquivalentTo("dotnet", "foo.dll");
  }

  [Fact]
  public void SetWorkingDir_Should_Set_Expected() {
    var sut = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    sut.SetWorkingDir("/foo-bar");
    sut.Config.WorkingDir.Should().BeEquivalentTo("/foo-bar");
  }
}