using System.Net.Http;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotnet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task UploadLayer(string repository, string image, ILayer layer);
  Task<Config> GetConfig(string repository, string image, Descriptor descriptor);
}

public class BlobOperations: IBlobOperations
{
  private readonly HttpClient _httpClient;

  public BlobOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public Task<Config> GetConfig(string repository, string image, Descriptor descriptor)
  {
    throw new System.NotImplementedException();
  }

  public async Task UploadLayer(string repository, string image, ILayer layer)
  {
    var locationUrl = string.Format("{0}/v2/{1}/blobs/uploads/", repository, image);
    var locationResponse = await _httpClient.PostAsync(locationUrl, null);
    locationResponse.EnsureSuccessStatusCode();
    var digest = layer.Digest();
    var uploadUri = string.Format("{0}&digest={1}", locationResponse.Headers.Location, digest);
    using var content = layer.Compressed();
    var streamContent = new StreamContent(content);
    var uploadResponse = await _httpClient.PutAsync(uploadUri, streamContent);
    uploadResponse.EnsureSuccessStatusCode();
  }
}
