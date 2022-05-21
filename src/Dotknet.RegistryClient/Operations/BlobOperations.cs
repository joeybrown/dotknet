using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotnet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task UploadLayer(string image, ILayer layer);
  Task<ConfigFile> GetConfig(string image, Descriptor descriptor);
}

public class BlobOperations: IBlobOperations
{
  private readonly HttpClient _httpClient;

  public BlobOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  private bool IsMcrImage(string image) => image.Contains("mcr.microsoft.com");

  public async Task<ConfigFile> GetConfig(string image, Descriptor descriptor)
  {
    if (IsMcrImage(image)){
      return await GetConfigFromMcr(image, descriptor);
    }

    throw new System.NotImplementedException();
  }

  public Task UploadLayer(string image, ILayer layer)
  {

    // var locationUrl = string.Format("{0}/v2/{1}/blobs/uploads/", repository, image);
    // var locationResponse = await _httpClient.PostAsync(locationUrl, null);
    // locationResponse.EnsureSuccessStatusCode();
    // var digest = layer.Digest();
    // var uploadUri = string.Format("{0}&digest={1}", locationResponse.Headers.Location, digest);
    // using var content = layer.Compressed();
    // var streamContent = new StreamContent(content);
    // var uploadResponse = await _httpClient.PutAsync(uploadUri, streamContent);
    // uploadResponse.EnsureSuccessStatusCode();
    return Task.CompletedTask;
  }

  private async Task<ConfigFile> GetConfigFromMcr(string image, Descriptor descriptor){
    var repo = image.Replace("mcr.microsoft.com", string.Empty);
    var endpoint = new Uri($"https://mcr.microsoft.com/v2/{repo}/blobs/{descriptor.Digest}");
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = endpoint
    };
    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();
    var str = await response.Content.ReadAsStringAsync();
    var content = await response.Content.ReadAsStreamAsync();
    var config = await JsonSerializer.DeserializeAsync<ConfigFile>(content);
    return config;
  }
}
