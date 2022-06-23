using System;
using System.Linq;
using Dotnet.RegistryClient.Models;
using FluentAssertions;
using Xunit;
using static Dotknet.Test.TestResources.TestResourceHelpers;

namespace Dotknet.Test.Models;

public class ConfigTests {
  [Fact]
  public void SetConfig_Should_SetExpected() {
    var sut = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    var originalEnv = sut.Config.Env?.ToArray() ?? Array.Empty<string>();
    var workingDir = "/foo-bar";
    var entryPoint = new [] {"dotnet", "foo.dll"};
    sut.SetConfig(workingDir, entryPoint);
    sut.Config.WorkingDir.Should().BeEquivalentTo(workingDir);
    sut.Config.Entrypoint.Should().BeEquivalentTo(entryPoint);
    sut.Config.Env.Should().BeEquivalentTo(originalEnv);
  }

  [Fact]
  public void AddHistory_Should_Add_Expected() {
    var sut = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    sut.History.Should().HaveCount(7);
    sut.AddHistory(new History{
      Created = new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc),
      CreatedBy = "dotknet",
      Comment = "Adding a dotnet app layer"
    });

    sut.History.Should().NotBeNull();
    sut.History.Should().HaveCount(8);

    var lastHistoryEntry = sut.History!.Last();
    lastHistoryEntry.CreatedBy.Should().BeEquivalentTo("dotknet");
    lastHistoryEntry.Comment.Should().BeEquivalentTo("Adding a dotnet app layer");
    lastHistoryEntry.Created.Should().Be(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
  }

  [Fact]
  public void SetCreated_Should_SetExpected(){
    var sut = ReadFromFile<ConfigFile>("DotnetRuntimeConfig.json");
    sut.SetCreated(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
    sut.Created.Should().Be(new DateTime(1970, 1, 1, 0, 0, 1, DateTimeKind.Utc));
  }
}
