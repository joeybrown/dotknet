namespace Dotknet.RegistryClient.Models.Manifests;

public class ImageManifestDocker2_2 : IImageManifest
{
  public int SchemaVersion {get;set;}

  public static ImageManifestDocker2_2 FromContent(string manifest){
    return new ImageManifestDocker2_2();
  }
}
