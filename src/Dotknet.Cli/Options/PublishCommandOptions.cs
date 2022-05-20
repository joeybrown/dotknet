namespace Dotknet.Cli.Options;

public class PublishCommandOptions
{
  public string? Project { get; set; }
  public string? Output { get; set; }
  public string? BaseImage { get; set; }
  public string LayerRoot => "dotknet-app";
  public string? DestinationImage {get;set;}
  public bool SkipDotnetBuild {get;set;}
}
