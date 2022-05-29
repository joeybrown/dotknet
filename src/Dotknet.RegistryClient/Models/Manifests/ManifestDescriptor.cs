namespace Dotknet.RegistryClient.Models.Manifests;

public class ManifestDescriptor
{
  public Descriptor Descriptor { get; set; }
  public IManifest Manifest { get; set; }

  public ManifestDescriptor(Descriptor descriptor, IManifest manifest)
  {
    Descriptor = descriptor;
    Manifest = manifest;
  }
}
