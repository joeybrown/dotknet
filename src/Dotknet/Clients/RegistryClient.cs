using System.Threading.Tasks;
using Dotknet.Models.Registry;
using Microsoft.Extensions.Logging;

namespace Dotknet.Clients;

public interface IRegistryClient {
  Task<ImageManifest> GetManifest(string imagePath); 
}

public class RegistryClient : IRegistryClient
{
  private readonly ILogger<RegistryClient> _logger;

  public RegistryClient(ILogger<RegistryClient> logger) {
    _logger = logger;
  }

  public Task<ImageManifest> GetManifest(string imagePath)
  {
    _logger.LogInformation($"Getting the manifest for {imagePath}");
    var manifest = new ImageManifest();
    return Task.FromResult(manifest);
  }
}
