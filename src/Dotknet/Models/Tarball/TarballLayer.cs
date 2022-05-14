using System;
using System.IO;
using Dotknet.RegistryClient;
using SharpCompress.Archives.Tar;

namespace Dotknet.Models.Tarball;

//todo: consider perf/caching
public class TarballLayer : ILayer
{
  // timestamp necessary for reproducibility
  public TarballLayer(TarArchive tarArchive, DateTime? modified = null)
  {
    _tarArchive = tarArchive;
    if (!modified.HasValue)
    {
      modified = new DateTime(1980, 01, 01, 00, 00, 01, DateTimeKind.Utc);
    }
    _modified = modified;
  }

  private DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

  private bool disposedValue;
  private readonly TarArchive _tarArchive;
  private readonly DateTime? _modified;

  public Stream Compressed()
  {
    using var originalStream = new MemoryStream();
    _tarArchive.SaveTo(originalStream, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.GZip));
    var buffer = originalStream.ToArray();
    var seconds = (int)(_modified! - EPOCH).Value.TotalSeconds;
    buffer[4] = (byte)seconds;
    buffer[5] = (byte)(seconds >> 8);
    buffer[6] = (byte)(seconds >> 16);
    buffer[7] = (byte)(seconds >> 24);
    return new MemoryStream(buffer);
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
    return Dotknet.RegistryClient.MediaType.DockerManifestSchema2;
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

  public override string ToString()
  {
    return $"Layer DiffId: {DiffId().Hex!.Substring(0, 6)} Digest: {Digest().Hex!.Substring(0, 6)}";
  }
}
