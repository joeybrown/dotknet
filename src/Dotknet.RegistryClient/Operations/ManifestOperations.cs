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
  // todo: We need a new type that is ManifestResponse because the response
  // could be a single manifest, or an index and we handle these differently
  Task<IImageManifest> GetManifest(string image);
  Task<IEnumerable<IImageManifest>> GetManifests(IImageManifest manifestList);
}

public class ManifestOperations : IManifestOperations
{
  private readonly HttpClient _httpClient;

  public ManifestOperations(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IImageManifest> GetManifest(string image)
  {
    var isDockerHubImage = !image.Contains("://");
    if (isDockerHubImage)
    {
      return await GetManifestFromDockerHub(image);
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

  public async Task<IImageManifest> GetManifestFromDockerHub(string image)
  {
    var endpoint = new Uri($"https://index.docker.io/v2/library/{image}/manifests/latest");
    var token = await GetDockerHubAuthToken(image);
    var requestMessage = new HttpRequestMessage
    {
      Method = HttpMethod.Get,
      RequestUri = endpoint
    };
    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    requestMessage.Headers.Add("Accept", MediaTypeEnum.Manifests.Select(x => x.Name));
    var response = await _httpClient.SendAsync(requestMessage);
    var content = await response.Content.ReadAsStringAsync();
    return ImageManifest.FromContent(content);
  }
}
