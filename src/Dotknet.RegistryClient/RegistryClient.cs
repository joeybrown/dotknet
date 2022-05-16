using System.Net.Http;
using Dotknet.RegistryClient.Operations;
using Microsoft.Extensions.Options;

namespace Dotknet.RegistryClient;

public interface IRegistryClient
{
  IManifestOperations ManifestOperations { get; }
  IBlobOperations BlobOperations {get;}
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
    BlobOperations = new BlobOperations(httpClient);
  }

  public IManifestOperations ManifestOperations { get; }

  public IBlobOperations BlobOperations {get;}
}
