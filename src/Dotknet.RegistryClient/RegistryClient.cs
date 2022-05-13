using System.Net.Http;
using Dotknet.RegistryClient.Operations;
using Microsoft.Extensions.Options;

namespace Dotknet.RegistryClient;

public interface IRegistryClient
{
  IManifestOperations ManifestOperations { get; }
}


public class RegistryClient : IRegistryClient
{
  private readonly HttpClient _httpClient;
  private readonly RegistryClientConfiguration _options;

  public RegistryClient(HttpClient httpClient, IOptions<RegistryClientConfiguration> options)
  {
    _httpClient = httpClient;
    _options = options.Value;
    ManifestOperations = new ManifestOperations(httpClient);
  }

  public IManifestOperations ManifestOperations { get; }

}
