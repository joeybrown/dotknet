using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Extensions;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;

namespace Dotknet.RegistryClient.Operations;

public interface IManifestOperations
{
  /// Get the manifest for the image. This manifest may be a manifest index.
  /// If the manifest is an index, it may be desirable to enumerate the
  /// manifests using <see cref="EnumerateManifests"/>.
  Task<IManifestRegistryResponse> GetManifest(IImageReference image);
  Task<IEnumerable<ManifestDescriptor>> EnumerateManifests(IImageReference image, IManifestIndex manifestIndex);
  Task<Hash> UploadManifest(IImageReference image, IManifestIndex manifestIndex);
  Task<Descriptor> UploadManifest(IImageReference image, IManifest manifest, Descriptor baseDescriptor);
}

public class ManifestOperations : IManifestOperations
{
  private readonly HttpClient _httpClient;

  public ManifestOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IManifestRegistryResponse> GetManifest(IImageReference image)
  {
    // This should be consolidated so we don't have this logic all over the place.
    if (image.IsMcrImage)
    {
      return await GetManifestFromMcr(image);
    }

    if (image.IsDockerHubImage)
    {
      var token = await GetDockerHubAuthToken(image);
      return await GetManifestFromDockerHub(image, token);
    }
    throw new System.NotImplementedException();
  }

  private async Task<IManifestRegistryResponse> GetManifestFromMcr(IImageReference image)
  {
    var endpoint = new Uri($"https://mcr.microsoft.com/v2/{image.Repository}/manifests/{image.Reference}");
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = endpoint
    };
    requestMessage.Headers.Add("Accept", MediaTypeEnum.Manifests.Select(x => x.Name));
    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var manifest = Manifest.FromContent(content);
    return manifest;
  }

  public async Task<IEnumerable<ManifestDescriptor>> EnumerateManifests(IImageReference image, IManifestIndex manifest)
  {
    if (image.IsMcrImage)
    {
      return await GetManifestsFromMcr(image, manifest.Manifests);
    }
    if (image.IsDockerHubImage)
    {
      var token = await GetDockerHubAuthToken(image);
      return await GetManifestsFromDockerHub(image, manifest.Manifests, token);
    }

    throw new System.NotImplementedException();
  }

  private class DockerHubAuth
  {
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
  }

  public async Task<string> GetDockerHubAuthToken(IImageReference image)
  {
    var endpoint = $"https://auth.docker.io/token?service=registry.docker.io&scope=repository:library/{image.Repository}:pull";
    var response = await _httpClient.GetAsync(endpoint);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStreamAsync();
    var dockerHubAuth = await JsonSerializer.DeserializeAsync<DockerHubAuth>(content);
    return dockerHubAuth!.AccessToken!;
  }

  private async Task<IManifestRegistryResponse> GetManifestFromDockerHub(IImageReference image, string token)
  {
    var endpoint = new Uri($"https://index.docker.io/v2/library/{image.Repository}/manifests/{image.Reference}");
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = endpoint
    };
    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    requestMessage.Headers.Add("Accept", MediaTypeEnum.Manifests.Select(x => x.Name));
    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var manifest = Manifest.FromContent(content);
    return manifest;
  }

  private async Task<IEnumerable<ManifestDescriptor>> GetManifestsFromDockerHub(IImageReference image, IEnumerable<Descriptor> descriptors, string token)
  {
    var tasks = descriptors.Select(async descriptor =>
    {
      var endpoint = new Uri($"https://index.docker.io/v2/library/{image.Repository}/manifests/{descriptor.Digest}");
      var requestMessage = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = endpoint
      };
      requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
      requestMessage.Headers.Add("Accept", MediaTypeEnum.Manifests.Select(x => x.Name));
      var response = await _httpClient.SendAsync(requestMessage);
      response.EnsureSuccessStatusCode();
      var content = await response.Content.ReadAsStringAsync();
      var manifest = Manifest.FromContent(content);
      return new ManifestDescriptor(descriptor, manifest as IManifest);
    }).ToArray();

    await Task.WhenAll(tasks);
    return tasks.Select(x => x.Result);
  }

  private async Task<IEnumerable<ManifestDescriptor>> GetManifestsFromMcr(IImageReference image, IEnumerable<Descriptor> descriptors)
  {
    var tasks = descriptors.Select(async descriptor =>
    {
      var endpoint = new Uri($"https://mcr.microsoft.com/v2/{image.Repository}/manifests/{descriptor.Digest}");
      var requestMessage = new HttpRequestMessage
      {
        Method = HttpMethod.Get,
        RequestUri = endpoint
      };
      requestMessage.Headers.Add("Accept", MediaTypeEnum.Manifests.Select(x => x.Name));
      var response = await _httpClient.SendAsync(requestMessage);
      response.EnsureSuccessStatusCode();
      var content = await response.Content.ReadAsStringAsync();
      var manifest = Manifest.FromContent(content);
      return new ManifestDescriptor(descriptor, manifest as IManifest);
    }).ToArray();

    await Task.WhenAll(tasks);
    return tasks.Select(x => x.Result);
  }

  public async Task<Hash> UploadManifest(IImageReference image, IManifestIndex manifest)
  {
    using var json = await manifest.ToJson();
    var digest = json.GetHash();
    json.Seek(0, SeekOrigin.Begin);

    var streamContent = new StreamContent(json);
    streamContent.Headers.Add("Content-Type", manifest.MediaType.Name);

    var endpoint = new Uri($"{image.Domain}/v2/{image.Repository}/manifests/{digest}");
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Put,
      RequestUri = endpoint,
      Content = streamContent
    };

    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();

    return digest;
  }

  public async Task<Descriptor> UploadManifest(IImageReference image, IManifest manifest, Descriptor baseDescriptor)
  {
    var manifestDescriptor = await manifest.BuildDescriptor(baseDescriptor);
    using var json = await manifest.ToJson();
    json.Seek(0, SeekOrigin.Begin);

    var streamContent = new StreamContent(json);
    streamContent.Headers.Add("Content-Type", manifest.MediaType.Name);

    var endpoint = new Uri($"{image.Domain}/v2/{image.Repository}/manifests/{manifestDescriptor.Digest}");
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Put,
      RequestUri = endpoint,
      Content = streamContent
    };
    var response = await _httpClient.SendAsync(requestMessage);
    response.EnsureSuccessStatusCode();
    var descriptor = await manifest.BuildDescriptor(baseDescriptor);
    return descriptor;
  }
}
