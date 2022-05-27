using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotnet.RegistryClient.Models;
using Microsoft.Extensions.Logging;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task CopyLayer(IImageReference sourceImage, IImageReference destinationImage, Hash digest);
  Task<(Descriptor descriptor, Hash diffId)> UploadLayer(IImageReference image, ILayer layer);
  Task<ConfigFile> GetConfig(IImageReference image, Descriptor descriptor);
  Task<Descriptor> UploadConfig(IImageReference image, ConfigFile config, Descriptor baseDescriptor);
  // Task<Descriptor> UploadManifest(IImageReference image, IManifest manifest, Descriptor baseDescriptor);
}

public class BlobOperations: IBlobOperations
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<BlobOperations> _logger;

  public BlobOperations(HttpClient httpClient, ILogger<BlobOperations> logger)
  {
    _httpClient = httpClient;
    _logger = logger;
  }

  public async Task<ConfigFile> GetConfig(IImageReference image, Descriptor descriptor)
  {
    if (image.IsMcrImage){
      return await GetConfigFromMcr(image, descriptor);
    }

    throw new System.NotImplementedException();
  }

  public async Task<Stream> GetLayer(IImageReference image, Hash digest)
  {
    if (image.IsMcrImage){
      return await GetLayerFromMcr(image, digest);
    }

    throw new System.NotImplementedException();
  }

  private async Task<Stream> GetLayerFromMcr(IImageReference image, Hash digest)
  {
    try {
    var uri = new Uri($"https://mcr.microsoft.com/v2/{image.Repository}/blobs/{digest}/");
    var response = await _httpClient.GetAsync(uri);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStreamAsync();
    }catch(Exception ex) {
      throw;
    }
  }

  private async Task UploadBlob(IImageReference image, Hash digest, StreamContent content){
    var locationUrl = new Uri($"{image.Domain}/v2/{image.Repository}/blobs/uploads/");
    var locationResponse = await _httpClient.PostAsync(locationUrl, null);
    locationResponse.EnsureSuccessStatusCode();
    var uploadUri = string.Format("{0}&digest={1}", locationResponse.Headers.Location, digest);
    var uploadResponse = await _httpClient.PutAsync(uploadUri, content);
    var response = await uploadResponse.Content.ReadAsStringAsync();
    uploadResponse.EnsureSuccessStatusCode();
  }

  public async Task<(Descriptor descriptor, Hash diffId)> UploadLayer(IImageReference image, ILayer layer)
  {
    var digest = layer.Digest();
    using var content = layer.Compressed();
    using var streamContent = new StreamContent(content);
    await UploadBlob(image, digest, streamContent);

    var descriptor = new Descriptor{
      MediaType = MediaTypeEnum.DockerUncompressedLayer,
      Size = layer.Size(),
      Digest = digest
    };
    var diffId = layer.DiffId();

    return (descriptor, diffId);
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
    using var streamContent = new StreamContent(content);
    var descriptor = await config.BuildDescriptor(baseDescriptor);
    await UploadBlob(image, descriptor.Digest, streamContent);
    return descriptor;
  }

  public async Task CopyLayer(IImageReference sourceImage, IImageReference destinationImage, Hash digest)
  {
    using var layer = await GetLayer(sourceImage, digest);
    _logger.LogInformation("Copying Layer {Digest}... Size {Size}", digest, layer.Length);
    layer.Seek(0, SeekOrigin.Begin);
    using var streamContent = new StreamContent(layer);
    await UploadBlob(destinationImage, digest, streamContent);
  }

  // public async Task<Descriptor> UploadManifest(IImageReference image, IManifest manifest, Descriptor baseDescriptor)
  // {
  //   using var json = await manifest.ToJson();
  //   json.Seek(0, SeekOrigin.Begin);
  //   var streamContent = new StreamContent(json);
  //   var descriptor = await manifest.BuildDescriptor(baseDescriptor);
  //   await UploadBlob(image, descriptor.Digest, streamContent);
  //   return descriptor;
  // }
}
