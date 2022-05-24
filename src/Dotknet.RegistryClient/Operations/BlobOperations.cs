using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotnet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task UploadLayer(IImageReference image, ILayer layer);
  Task<ConfigFile> GetConfig(IImageReference image, Descriptor descriptor);
  Task<Descriptor> UploadConfig(IImageReference image, ConfigFile config, Descriptor baseDescriptor);
}

public class BlobOperations: IBlobOperations
{
  private readonly HttpClient _httpClient;

  public BlobOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<ConfigFile> GetConfig(IImageReference image, Descriptor descriptor)
  {
    if (image.IsMcrImage){
      return await GetConfigFromMcr(image, descriptor);
    }

    throw new System.NotImplementedException();
  }

  private async Task UploadBlob(IImageReference image, Hash digest, StreamContent content){
    var locationUrl = new Uri($"{image.Domain}/v2/{image.Repository}/blobs/uploads/");
    var locationResponse = await _httpClient.PostAsync(locationUrl, null);
    locationResponse.EnsureSuccessStatusCode();
    var uploadUri = string.Format("{0}&digest={1}", locationResponse.Headers.Location, digest);
    var uploadResponse = await _httpClient.PutAsync(uploadUri, content);
    uploadResponse.EnsureSuccessStatusCode();
  }

  public async Task UploadLayer(IImageReference image, ILayer layer)
  {
    var digest = layer.Digest();
    using var content = layer.Compressed();
    using var streamContent = new StreamContent(content);
    await UploadBlob(image, layer.Digest(), streamContent);
  }

  private async Task<ConfigFile> GetConfigFromMcr(IImageReference image, Descriptor descriptor){    
    var endpoint = new Uri($"https://mcr.microsoft.com/v2/{image.Repository}/blobs/{descriptor.Digest}");
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

  public async Task<Descriptor> UploadConfig(IImageReference image, ConfigFile config, Descriptor baseDescriptor)
  {
    using var content = new MemoryStream();
    await JsonSerializer.SerializeAsync(content, config);
    content.Seek(0, SeekOrigin.Begin);
    var streamContent = new StreamContent(content);
    var descriptor = await config.BuildDescriptor(baseDescriptor);
    await UploadBlob(image, descriptor.Digest, streamContent);
    return descriptor;
  }
}
