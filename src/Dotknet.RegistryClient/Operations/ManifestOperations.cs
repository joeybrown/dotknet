using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dotknet.RegistryClient.Models;
using Dotknet.RegistryClient.Models.Manifests;

namespace Dotknet.RegistryClient.Operations;

public interface IManifestOperations
{
  /// Get the manifest for the image. This manifest may be a manifest index.
  /// If the manifest is an index, it may be desirable to enumerate the
  /// manifests using <see cref="EnumerateManifests"/>.
  Task<IManifestRegistryResponse> GetManifest(string image);
  Task<IEnumerable<ManifestDescriptor>> EnumerateManifests(string image, IManifestIndex manifestIndex);
}

public class ManifestOperations : IManifestOperations
{
  private readonly HttpClient _httpClient;

  public ManifestOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  private bool IsDockerHubImage(string image) => !image.Contains("://");

  public async Task<IManifestRegistryResponse> GetManifest(string image)
  {
    if (IsDockerHubImage(image))
    {
      var token = await GetDockerHubAuthToken(image);
      return await GetManifestFromDockerHub(image, token);
    }
    throw new System.NotImplementedException();
  }

  public async Task<IEnumerable<ManifestDescriptor>> EnumerateManifests(string image, IManifestIndex manifest)
  {
    if (IsDockerHubImage(image))
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

  public async Task<string> GetDockerHubAuthToken(string image)
  {
    var endpoint = $"https://auth.docker.io/token?service=registry.docker.io&scope=repository:library/{image}:pull";
    var response = await _httpClient.GetAsync(endpoint);
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStreamAsync();
    var dockerHubAuth = await JsonSerializer.DeserializeAsync<DockerHubAuth>(content);
    return dockerHubAuth!.AccessToken!;
  }

  private async Task<IManifestRegistryResponse> GetManifestFromDockerHub(string image, string token)
  {
    var endpoint = new Uri($"https://index.docker.io/v2/library/{image}/manifests/latest");
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

  private async Task<IEnumerable<ManifestDescriptor>> GetManifestsFromDockerHub(string image, IEnumerable<Descriptor> descriptors, string token)
  {
    var tasks = descriptors.Select(async descriptor =>
    {
      var endpoint = new Uri($"https://index.docker.io/v2/library/{image}/manifests/{descriptor.Digest}");
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
      var manifest = (IManifest) Manifest.FromContent(content);
      return new ManifestDescriptor(descriptor, manifest);
    }).ToArray();

    await Task.WhenAll(tasks);
    return tasks.Select(x => x.Result);
  }
}
