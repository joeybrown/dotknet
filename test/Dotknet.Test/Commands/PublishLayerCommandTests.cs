using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Xunit;

namespace Dotknet.Test.Commands;

public class PublishLayerCommandTests
{
  [Fact]
  public async Task PublishToDaemonWorks()
  {
    var uri = new Uri("https://mcr.microsoft.com/v2");
    using var client = new DockerClientConfiguration(uri).CreateClient();
    var @params = new Docker.DotNet.Models.ImagesListParameters{All=true};
    var images = await client.Images.ListImagesAsync(@params);
    Console.WriteLine($"Image Count {images.Count}");
  }
}
