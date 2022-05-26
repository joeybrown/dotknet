using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;
using Dotnet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task<Descriptor> CopyLayer(IImageReference sourceImage, IImageReference destinationImage, Descriptor descriptor);
  Task<(Descriptor descriptor, Hash diffId)> UploadLayer(IImageReference image, ILayer layer);
  Task<ConfigFile> GetConfig(IImageReference image, Descriptor descriptor);
  Task<Descriptor> UploadConfig(IImageReference image, ConfigFile config, Descriptor baseDescriptor);
  // Task<Descriptor> UploadManifest(IImageReference image, IManifest manifest, Descriptor baseDescriptor);
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
    var streamContent = new StreamContent(content);
    var descriptor = await config.BuildDescriptor(baseDescriptor);
    await UploadBlob(image, descriptor.Digest, streamContent);
    return descriptor;
  }

  public Task<Descriptor> CopyLayer(IImageReference sourceImage, IImageReference destinationImage, Descriptor descriptor)
  {
    // todo :)
    throw new NotImplementedException();
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
