namespace Dotknet.Commands;

public class LifecycleOptions
{
  public string? Project { get; set; }
  public string? Output { get; set; }
  public string? DirectoryToArchive { get; set; }
  public string LayerAppRoot => "dotknet-app";
}
