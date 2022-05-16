using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Operations;

public interface IBlobOperations
{
  Task<Hash> UploadBlob(string repository, string image, ILayer layer);
}

public class BlobOperations: IBlobOperations
{
  private readonly HttpClient _httpClient;

  public BlobOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Hash> UploadBlob(string repository, string image, ILayer layer)
  {
    var locationUrl = string.Format("{0}/v2/{1}/blobs/uploads/", repository, image);
    var locationResponse = await _httpClient.PostAsync(locationUrl, null);
    
    var digest = layer.Digest();

    var uploadUri = string.Format("{0}?digest={1}", locationResponse.Headers.Location, digest);

    using var content = layer.Uncompressed();
    var streamContent = new StreamContent(content);
    var uploadResponse = await _httpClient.PutAsync(uploadUri, streamContent);

    return new Hash();
  }
}