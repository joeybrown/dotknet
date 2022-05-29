using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Dotknet.RegistryClient.Models;

[JsonConverter(typeof(SmartEnumNameConverter<MediaTypeEnum, int>))]
public abstract class MediaTypeEnum : SmartEnum<MediaTypeEnum>
{
  public static readonly MediaTypeEnum OCIContentDescriptor = new OCIContentDescriptorType();
  public static readonly MediaTypeEnum OCIImageIndex = new OCIImageIndexType();
  public static readonly MediaTypeEnum OCIConfigJSON = new OCIConfigJSONType();
  public static readonly MediaTypeEnum OCILayer = new OCILayerType();
  public static readonly MediaTypeEnum OCIRestrictedLayer = new OCIRestrictedLayerType();
  public static readonly MediaTypeEnum OCIUncompressedLayer = new OCIUncompressedLayerType();
  public static readonly MediaTypeEnum OCIUncompressedRestrictedLayer = new OCIUncompressedRestrictedLayerType();
  public static readonly MediaTypeEnum DockerManifestSchema1 = new DockerManifestSchema1Type();
  public static readonly MediaTypeEnum DockerManifestSchema1Signed = new DockerManifestSchema1SignedType();
  public static readonly MediaTypeEnum DockerManifestSchema2 = new DockerManifestSchema2Type();
  public static readonly MediaTypeEnum DockerManifestList = new DockerManifestListType();
  public static readonly MediaTypeEnum DockerLayer = new DockerLayerType();
  public static readonly MediaTypeEnum DockerConfigJSON = new DockerConfigJSONType();
  public static readonly MediaTypeEnum DockerPluginConfig = new DockerPluginConfigType();
  public static readonly MediaTypeEnum DockerForeignLayer = new DockerForeignLayerType();
  public static readonly MediaTypeEnum DockerUncompressedLayer = new DockerUncompressedLayerType();
  public static readonly MediaTypeEnum OCIVendorPrefix = new OCIVendorPrefixType();
  public static readonly MediaTypeEnum DockerVendorPrefix = new DockerVendorPrefixType();

  private MediaTypeEnum(string name, int value) : base(name, value)
  {
  }

  public abstract bool IsManifest { get; }
  public abstract bool IsManifestIndex { get; }

  private class OCIContentDescriptorType : MediaTypeEnum
  {
    public OCIContentDescriptorType() : base("application/vnd.oci.descriptor.v1+json", 1) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCIImageIndexType : MediaTypeEnum
  {
    public OCIImageIndexType() : base("application/vnd.oci.image.index.v1+json", 2) { }
    public override bool IsManifest => true;
    public override bool IsManifestIndex => true;
  }

  private class OCIConfigJSONType : MediaTypeEnum
  {
    public OCIConfigJSONType() : base("application/vnd.oci.image.config.v1+json", 4) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCILayerType : MediaTypeEnum
  {
    public OCILayerType() : base("application/vnd.oci.image.layer.v1.tar+gzip", 5) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCIRestrictedLayerType : MediaTypeEnum
  {
    public OCIRestrictedLayerType() : base("application/vnd.oci.image.layer.nondistributable.v1.tar+gzip", 6) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCIUncompressedLayerType : MediaTypeEnum
  {
    public OCIUncompressedLayerType() : base("application/vnd.oci.image.layer.v1.tar", 7) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCIUncompressedRestrictedLayerType : MediaTypeEnum
  {
    public OCIUncompressedRestrictedLayerType() : base("application/vnd.oci.image.layer.nondistributable.v1.tar", 8) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerManifestSchema1Type : MediaTypeEnum
  {
    public DockerManifestSchema1Type() : base("application/vnd.docker.distribution.manifest.v1+json", 9) { }
    public override bool IsManifest => true;
    public override bool IsManifestIndex => false;
  }

  private class DockerManifestSchema1SignedType : MediaTypeEnum
  {
    public DockerManifestSchema1SignedType() : base("application/vnd.docker.distribution.manifest.v1+prettyjws", 10) { }
    public override bool IsManifest => true;
    public override bool IsManifestIndex => false;
  }

  private class DockerManifestSchema2Type : MediaTypeEnum
  {
    public DockerManifestSchema2Type() : base("application/vnd.docker.distribution.manifest.v2+json", 11) { }
    public override bool IsManifest => true;
    public override bool IsManifestIndex => false;
  }

  private class DockerManifestListType : MediaTypeEnum
  {
    public DockerManifestListType() : base("application/vnd.docker.distribution.manifest.list.v2+json", 12) { }
    public override bool IsManifest => true;
    public override bool IsManifestIndex => true;
  }

  private class DockerLayerType : MediaTypeEnum
  {
    public DockerLayerType() : base("application/vnd.docker.image.rootfs.diff.tar.gzip", 13) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerConfigJSONType : MediaTypeEnum
  {
    public DockerConfigJSONType() : base("application/vnd.docker.container.image.v1+json", 14) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerPluginConfigType : MediaTypeEnum
  {
    public DockerPluginConfigType() : base("application/vnd.docker.plugin.v1+json", 15) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerForeignLayerType : MediaTypeEnum
  {
    public DockerForeignLayerType() : base("application/vnd.docker.image.rootfs.foreign.diff.tar.gzip", 16) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerUncompressedLayerType : MediaTypeEnum
  {
    public DockerUncompressedLayerType() : base("application/vnd.docker.image.rootfs.diff.tar", 17) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class OCIVendorPrefixType : MediaTypeEnum
  {
    public OCIVendorPrefixType() : base("vnd.oci", 18) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  private class DockerVendorPrefixType : MediaTypeEnum
  {
    public DockerVendorPrefixType() : base("vnd.docker", 19) { }
    public override bool IsManifest => false;
    public override bool IsManifestIndex => false;
  }

  public static IEnumerable<MediaTypeEnum> Manifests =>
    MediaTypeEnum.List.Where(x => x.IsManifest);
}

// public static class MediaTypeExtensions
// {
//   // IsDistributable returns true if a layer is distributable, see:
//   // https://github.com/opencontainers/image-spec/blob/master/layer.md#non-distributable-layers
//   public static bool IsDistributable(this MediaType mediaType)
//   {
//     switch (mediaType)
//     {
//       case MediaType.DockerForeignLayer:
//       case MediaType.OCIRestrictedLayer:
//       case MediaType.OCIUncompressedLayer:
//         return true;
//       default:
//         return false;
//     }
//   }
//   // IsImage returns true if the mediaType represents an image manifest, as opposed to something else, like an index.
//   public static bool IsImage(this MediaType mediaType)
//   {
//     switch (mediaType)
//     {
//       case MediaType.OCIManifestSchema1:
//       case MediaType.DockerManifestSchema2:
//         return true;
//       default:
//         return false;
//     }
//   }
//   // IsIndex returns true if the mediaType represents an index, as opposed to something else, like an image.
//   public static bool IsIndex(this MediaType mediaType)
//   {
//     switch (mediaType)
//     {
//       case MediaType.OCIImageIndex:
//       case MediaType.DockerManifestList:
//         return true;
//       default:
//         return false;
//     }
//   }
// }
