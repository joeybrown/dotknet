namespace Dotknet.RegistryClient;

public interface IImageReference
{
  string Domain { get; set; }
  string Repository { get; set; }
  string Reference { get; set; }
  bool IsMcrImage {get;}
  bool IsDockerHubImage {get;}
}

public class ImageReference : IImageReference
{
  public string Domain { get; set; }
  public string Repository { get; set; }
  public string Reference { get; set; } = "latest";
  public bool IsMcrImage => Domain.Contains("mcr.microsoft.com");
  public bool IsDockerHubImage => Domain.Contains("docker.io");
}
