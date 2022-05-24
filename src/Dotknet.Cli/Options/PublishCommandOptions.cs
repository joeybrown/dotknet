namespace Dotknet.Cli.Options;

public class PublishCommandOptions
{
  public string? Project { get; set; }
  public string? Output { get; set; }
  public string? BaseImageDomain { get; set; }
  public string? BaseImageRepository { get; set; }
  public string LayerRoot => "dotknet-app";
  public string? DestinationImageDomain { get; set; }
  public string? DestinationImageRepository { get; set; }
  public bool SkipDotnetBuild {get;set;}
}
