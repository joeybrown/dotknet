using System;
using System.Linq;
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
}