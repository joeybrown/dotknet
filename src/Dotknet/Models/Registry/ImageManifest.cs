using System.Collections.Generic;

namespace Dotknet.Models.Registry;

public class ImageManifest
{
  public ImageConfiguration Config { get; set; }
  public IEnumerable<ImageLayer> ImageLayers { get; set; }
}
