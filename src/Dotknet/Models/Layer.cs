using Dotknet.Extensions;
using SharpCompress.Archives.Tar;

namespace Dotknet.Models;

/// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/layer.go
public interface ILayer: IDisposable
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

//todo: consider perf/caching
public class TarArchiveLayer : ILayer
{
  public TarArchiveLayer(TarArchive tarArchive)
  {
    _tarArchive = tarArchive;
  }

  private bool disposedValue;
  private readonly TarArchive _tarArchive;

  public Stream Compressed()
  {
    var memoryStream = new MemoryStream();
    _tarArchive.SaveTo(memoryStream, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.GZip));
    return memoryStream;
  }

  public Hash DiffId()
  {
    using var stream = Uncompressed();
    return stream.GetHash();
  }

  public Hash Digest()
  {
    using var stream = Compressed();
    return stream.GetHash();
  }

  public MediaType MediaType()
  {
    return Dotknet.Models.MediaType.DockerManifestSchema2;
  }

  public long Size()
  {
    using var stream = Compressed();
    return stream.Length;
  }

  public Stream Uncompressed()
  {
    var memoryStream = new MemoryStream();
    _tarArchive.SaveTo(memoryStream, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None));
    return memoryStream;
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!disposedValue)
    {
      _tarArchive.Dispose();
      disposedValue = true;
    }
  }

  public void Dispose()
  {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  public override string ToString(){
    return $"Layer DiffId: {DiffId().Hex.Substring(0, 6)} Digest: {Digest().Hex.Substring(0, 6)}";
  }
}