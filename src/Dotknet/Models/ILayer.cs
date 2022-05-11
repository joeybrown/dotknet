using System;
using System.IO;

namespace Dotknet.Models;

/// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/layer.go
public interface ILayer : IDisposable
{
  // Digest returns the Hash of the compressed layer.
  public Hash Digest();

  // DiffID returns the Hash of the uncompressed layer.
  public Hash DiffId();

  // Compressed returns a stream for the compressed layer contents.
  public Stream Compressed();

  // Uncompressed returns a stream for the uncompressed layer contents.
  public Stream Uncompressed();

  // Size returns the compressed size of the Layer.
  public long Size();

  // MediaType returns the media type of the Layer.
  public MediaType MediaType();
}
