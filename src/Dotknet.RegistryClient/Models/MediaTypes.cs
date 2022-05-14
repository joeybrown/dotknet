using System.Collections.Generic;
using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Dotknet.RegistryClient.Models;

[JsonConverter(typeof(SmartEnumNameConverter<MediaTypeEnum, int>))]
public sealed class MediaTypeEnum : SmartEnum<MediaTypeEnum>
{
  public static readonly MediaTypeEnum OCIContentDescriptor = new MediaTypeEnum("application/vnd.oci.descriptor.v1+json", 1);
  public static readonly MediaTypeEnum OCIImageIndex = new MediaTypeEnum("application/vnd.oci.image.index.v1+json", 2);
  public static readonly MediaTypeEnum OCIManifestSchema1 = new MediaTypeEnum("application/vnd.oci.image.manifest.v1+json", 3);
  public static readonly MediaTypeEnum OCIConfigJSON = new MediaTypeEnum("application/vnd.oci.image.config.v1+json", 4);
  public static readonly MediaTypeEnum OCILayer = new MediaTypeEnum("application/vnd.oci.image.layer.v1.tar+gzip", 5);
  public static readonly MediaTypeEnum OCIRestrictedLayer = new MediaTypeEnum("application/vnd.oci.image.layer.nondistributable.v1.tar+gzip", 6);
  public static readonly MediaTypeEnum OCIUncompressedLayer = new MediaTypeEnum("application/vnd.oci.image.layer.v1.tar", 7);
  public static readonly MediaTypeEnum OCIUncompressedRestrictedLayer = new MediaTypeEnum("application/vnd.oci.image.layer.nondistributable.v1.tar", 8);
  public static readonly MediaTypeEnum DockerManifestSchema1 = new MediaTypeEnum("application/vnd.docker.distribution.manifest.v1+json", 9);
  public static readonly MediaTypeEnum DockerManifestSchema1Signed = new MediaTypeEnum("application/vnd.docker.distribution.manifest.v1+prettyjws", 10);
  public static readonly MediaTypeEnum DockerManifestSchema2 = new MediaTypeEnum("application/vnd.docker.distribution.manifest.v2+json", 11);
  public static readonly MediaTypeEnum DockerManifestList = new MediaTypeEnum("application/vnd.docker.distribution.manifest.list.v2+json", 12);
  public static readonly MediaTypeEnum DockerLayer = new MediaTypeEnum("application/vnd.docker.image.rootfs.diff.tar.gzip", 13);
  public static readonly MediaTypeEnum DockerConfigJSON = new MediaTypeEnum("application/vnd.docker.container.image.v1+json", 14);
  public static readonly MediaTypeEnum DockerPluginConfig = new MediaTypeEnum("application/vnd.docker.plugin.v1+json", 15);
  public static readonly MediaTypeEnum DockerForeignLayer = new MediaTypeEnum("application/vnd.docker.image.rootfs.foreign.diff.tar.gzip", 16);
  public static readonly MediaTypeEnum DockerUncompressedLayer = new MediaTypeEnum("application/vnd.docker.image.rootfs.diff.tar", 17);
  public static readonly MediaTypeEnum OCIVendorPrefix = new MediaTypeEnum("vnd.oci", 18);
  public static readonly MediaTypeEnum DockerVendorPrefix = new MediaTypeEnum("vnd.docker", 19);

  private MediaTypeEnum(string name, int value) : base(name, value)
  {
  }

  public static IEnumerable<MediaTypeEnum> Manifests => new[] {
    MediaTypeEnum.OCIImageIndex,
    MediaTypeEnum.DockerManifestList,
    MediaTypeEnum.OCIManifestSchema1,
    MediaTypeEnum.DockerManifestSchema1,
    MediaTypeEnum.DockerManifestSchema1Signed,
    MediaTypeEnum.DockerManifestSchema2
  };
}

// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/types/types.go
//MediaType is an enumeration of the supported mime types that an element of an image might have.
// public enum MediaType
// {
//   [Description("application/vnd.oci.descriptor.v1+json")]
//   OCIContentDescriptor,

//   [Description("application/vnd.oci.image.index.v1+json")]
//   OCIImageIndex,

//   [Description("application/vnd.oci.image.manifest.v1+json")]
//   OCIManifestSchema1,

//   [Description("application/vnd.oci.image.config.v1+json")]
//   OCIConfigJSON,

//   [Description("application/vnd.oci.image.layer.v1.tar+gzip")]
//   OCILayer,

//   [Description("application/vnd.oci.image.layer.nondistributable.v1.tar+gzip")]
//   OCIRestrictedLayer,

//   [Description("application/vnd.oci.image.layer.v1.tar")]
//   OCIUncompressedLayer,

//   [Description("application/vnd.oci.image.layer.nondistributable.v1.tar")]
//   OCIUncompressedRestrictedLayer,


//   [Description("application/vnd.docker.distribution.manifest.v1+json")]
//   DockerManifestSchema1,

//   [Description("application/vnd.docker.distribution.manifest.v1+prettyjws")]
//   DockerManifestSchema1Signed,

//   [Description("application/vnd.docker.distribution.manifest.v2+json")]
//   DockerManifestSchema2,

//   [Description("application/vnd.docker.distribution.manifest.list.v2+json")]
//   DockerManifestList,

//   [Description("application/vnd.docker.image.rootfs.diff.tar.gzip")]
//   DockerLayer,

//   [Description("application/vnd.docker.container.image.v1+json")]
//   DockerConfigJSON,

//   [Description("application/vnd.docker.plugin.v1+json")]
//   DockerPluginConfig,

//   [Description("application/vnd.docker.image.rootfs.foreign.diff.tar.gzip")]
//   DockerForeignLayer,

//   [Description("application/vnd.docker.image.rootfs.diff.tar")]
//   DockerUncompressedLayer,


//   [Description("vnd.oci")]
//   OCIVendorPrefix,

//   [Description("vnd.docker")]
//   DockerVendorPrefix
// }

// public static class MediaTypeExtensions
// {

//   public static string Description(this MediaType source)
//   {
//     var fi = source.GetType().GetField(source.ToString());

//     DescriptionAttribute[] attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(
//         typeof(DescriptionAttribute), false);

//     if (attributes != null && attributes.Length > 0) return attributes[0].Description;
//     else return source.ToString();
//   }

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
