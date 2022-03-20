using Xunit;
using DotKnet.Core.Models;
using FluentAssertions;
using System.Linq;

namespace DotKnet.Console.Test;

public class ManifestTests
{
  private readonly Manifest RealManifest;

  public ManifestTests()
  {
    var manifestJson = "{\"schemaVersion\": 2,\"mediaType\": \"application/vnd.docker.distribution.manifest.v2+json\",\"config\": {\"mediaType\": \"application/vnd.docker.container.image.v1+json\",\"size\": 1678,\"digest\": \"sha256:c653c9a2bd72c4e81047419e63fbd5c7800e3367672930c55e49c87038ea934b\"},\"layers\": [{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 26708326,\"digest\": \"sha256:cf06a7c3161117888114e7e91dbd21915efae33c2dbfb086380f7b21946d6e59\"},{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 126,\"digest\": \"sha256:7da0dc299d86e304a2594750b932c747c08c84828112f83f81376ab3e934d880\"},{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 125,\"digest\": \"sha256:84d910c8cd9f6862398d6720d1819f2fbaee0cbfa9b9042b31483ac2fcec28fe\"},{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 142,\"digest\": \"sha256:05f521034a49b13f0b531c86967d949376aabcb627669409fa3b392ad1d50fa2\"},{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 148,\"digest\": \"sha256:534480d7d030253519cd058989c086da9bd21e9771b7fe15328174939600fb17\"},{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\",\"size\": 124,\"digest\": \"sha256:be6331389b612cf4bee2a9417189b42729f857ba93be613acb6440dfabbe968a\"}]}";
    RealManifest = Manifest.FromJson(manifestJson);
  }

  [Fact]
  public void No_Layers_In_Json_Should_Get_No_Layers_In_Obj()
  {
    var json = "{\"config\": {\"mediaType\": \"application/vnd.docker.container.image.v1+json\",\"size\": 1678,\"digest\": \"sha256:c653c9a2bd72c4e81047419e63fbd5c7800e3367672930c55e49c87038ea934b\"}, \"layers\": []}";
    var manifest = Manifest.FromJson(json);
    manifest.Layers.Should().BeEmpty();
    manifest.Config.Should().NotBeNull();
  }

  [Fact]
  public void One_Layer_In_Json_Should_Get_One_Layer_In_Obj()
  {
    var json = "{\"config\": {\"mediaType\": \"application/vnd.docker.container.image.v1+json\",\"size\": 1678,\"digest\": \"sha256:c653c9a2bd72c4e81047419e63fbd5c7800e3367672930c55e49c87038ea934b\"}, \"layers\": [{\"mediaType\": \"application/vnd.docker.image.rootfs.diff.tar.gzip\", \"size\": 500, \"digest\": \"sha:1234\"}]}";
    var manifest = Manifest.FromJson(json);

    var layer = manifest.Layers.Single();
    layer.Digest.Algorithm.Should().Be("sha");
    layer.Digest.Hex.Should().Be("1234");
    layer.MediaType.Should().Be(MediaType.Layer);
    layer.Size.Should().Be(500);
    manifest.Config.MediaType.Should().Be(MediaType.ContainerConfigJson);
    manifest.Config.Size.Should().Be(1678);
    manifest.Config.Digest.Algorithm.Should().Be("sha256");
    manifest.Config.Digest.Hex.Should().Be("c653c9a2bd72c4e81047419e63fbd5c7800e3367672930c55e49c87038ea934b");
  }

  [Fact]
  public void Real_Manifest_Should_Parse()
  {
    RealManifest.Layers.Should().HaveCount(6);
  }

  [Fact]
  public void Should_Add_Layer_Successfully()
  {
    var config = new ConfigDescriptor(555, "sha256:1234");
    var layer = new LayerDescriptor(9992, "sha256:99999");
    var manifest = RealManifest.AddLayer(layer);
  }
}