using System;
using System.Threading.Tasks;
using Docker.Registry.DotNet;
using Dotknet.Models.Registry;
using Microsoft.Extensions.Logging;
using ImageManifest = Dotknet.Models.Registry.ImageManifest;

namespace Dotknet.Clients;

public interface IRegistryClient
{
  Task<IImageManifest> GetManifest(string imagePath);
}

public class RegistryClient : IRegistryClient
{
  private readonly ILogger<RegistryClient> _logger;

  public RegistryClient(ILogger<RegistryClient> logger)
  {
    _logger = logger;
  }

  public async Task<IImageManifest> GetManifest(string imagePath)
  {
    _logger.LogInformation($"Getting the manifest for {imagePath}");
    var uri = new Uri(imagePath);
    var registryHost = uri.GetLeftPart(UriPartial.Authority);
    var imageName = uri.AbsolutePath.Substring(1);

    var configuration = new RegistryClientConfiguration(registryHost);
    using var client = configuration.CreateClient();
    var result = await client.Manifest.GetManifestAsync(imageName, "latest");

    var manifest = ImageManifest.FromContent(result.Content);

    return manifest;
  }
}
