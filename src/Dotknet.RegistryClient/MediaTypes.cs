
using System.ComponentModel;

namespace Dotknet.RegistryClient;

/// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/types/types.go
// MediaType is an enumeration of the supported mime types that an element of an image might have.
public enum MediaType
{
  [Description("application/vnd.oci.descriptor.v1+json")]
  OCIContentDescriptor,

  [Description("application/vnd.oci.image.index.v1+json")]
  OCIImageIndex,

  [Description("application/vnd.oci.image.manifest.v1+json")]
  OCIManifestSchema1,

  [Description("application/vnd.oci.image.config.v1+json")]
  OCIConfigJSON,

  [Description("application/vnd.oci.image.layer.v1.tar+gzip")]
  OCILayer,

  [Description("application/vnd.oci.image.layer.nondistributable.v1.tar+gzip")]
  OCIRestrictedLayer,

  [Description("application/vnd.oci.image.layer.v1.tar")]
  OCIUncompressedLayer,

  [Description("application/vnd.oci.image.layer.nondistributable.v1.tar")]
  OCIUncompressedRestrictedLayer,


  [Description("application/vnd.docker.distribution.manifest.v1+json")]
  DockerManifestSchema1,

  [Description("application/vnd.docker.distribution.manifest.v1+prettyjws")]
  DockerManifestSchema1Signed,

  [Description("application/vnd.docker.distribution.manifest.v2+json")]
  DockerManifestSchema2,

  [Description("application/vnd.docker.distribution.manifest.list.v2+json")]
  DockerManifestList,

  [Description("application/vnd.docker.image.rootfs.diff.tar.gzip")]
  DockerLayer,

  [Description("application/vnd.docker.container.image.v1+json")]
  DockerConfigJSON,

  [Description("application/vnd.docker.plugin.v1+json")]
  DockerPluginConfig,

  [Description("application/vnd.docker.image.rootfs.foreign.diff.tar.gzip")]
  DockerForeignLayer,

  [Description("application/vnd.docker.image.rootfs.diff.tar")]
  DockerUncompressedLayer,


  [Description("vnd.oci")]
  OCIVendorPrefix,

  [Description("vnd.docker")]
  DockerVendorPrefix
}

public static class MediaTypeExtensions
{

  public static string Description(this MediaType source)
  {
    var fi = source.GetType().GetField(source.ToString());

    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(
        typeof(DescriptionAttribute), false);

    if (attributes != null && attributes.Length > 0) return attributes[0].Description;
    else return source.ToString();
  }

  // IsDistributable returns true if a layer is distributable, see:
  // https://github.com/opencontainers/image-spec/blob/master/layer.md#non-distributable-layers
  public static bool IsDistributable(this MediaType mediaType)
  {
    switch (mediaType)
    {
      case MediaType.DockerForeignLayer:
      case MediaType.OCIRestrictedLayer:
      case MediaType.OCIUncompressedLayer:
        return true;
      default:
        return false;
    }
  }

  // IsImage returns true if the mediaType represents an image manifest, as opposed to something else, like an index.
  public static bool IsImage(this MediaType mediaType)
  {
    switch (mediaType)
    {
      case MediaType.OCIManifestSchema1:
      case MediaType.DockerManifestSchema2:
        return true;
      default:
        return false;
    }
  }

  // IsIndex returns true if the mediaType represents an index, as opposed to something else, like an image.
  public static bool IsIndex(this MediaType mediaType)
  {
    switch (mediaType)
    {
      case MediaType.OCIImageIndex:
      case MediaType.DockerManifestList:
        return true;
      default:
        return false;
    }
  }
}
