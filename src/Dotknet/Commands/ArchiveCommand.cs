using Microsoft.Extensions.Options;
using SharpCompress.Archives.Tar;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using Dotknet.Extensions;
using Dotknet.Models;

namespace Dotknet.Commands;

public class ArchiveCommandOptions
{
  public string? Output { get; set; }
  public string? SourceDirectory { get; set; }
  public string LayerRoot => "dotknet-app";
}

public interface IArchiveCommand
{
  void Execute();
}

/// It would be cool if dotnet had native APIs for this.
/// https://github.com/dotnet/runtime/issues/65951
public class ArchiveCommand : IArchiveCommand
{
  private readonly ArchiveCommandOptions _options;
  private readonly ILogger<ArchiveCommand> _logger;
  private readonly WriterOptions _uncompressed;
  private readonly WriterOptions _compressed;

  public ArchiveCommand(IOptions<ArchiveCommandOptions> options, ILogger<ArchiveCommand> logger)
  {
    _options = options.Value;
    _logger = logger;
    _uncompressed = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None);
    _compressed = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.GZip);
  }

  public void Execute()
  {
    using var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory(_options.SourceDirectory!, _options.LayerRoot);

    using var layer = new TarArchiveLayer(tarArchive);

    var uncompressedDestination = Path.Join(_options.Output!, layer.DiffId().Hex! + ".tar");
    using var uncompressedFileStream = new FileStream(uncompressedDestination!, FileMode.Create, FileAccess.Write);
    using var uncompressedStream = layer.Uncompressed();
    uncompressedStream.Seek(0, SeekOrigin.Begin);
    uncompressedStream.CopyTo(uncompressedFileStream);

    var compressedDestination = Path.Join(_options.Output!, layer.Digest().Hex! + ".tar.gz");
    using var compressedFileStream = new FileStream(compressedDestination!, FileMode.Create, FileAccess.Write);
    using var compressedStream = layer.Compressed();
    compressedStream.Seek(0, SeekOrigin.Begin);
    compressedStream.CopyTo(compressedFileStream);

    _logger.LogInformation("DiffId: {DiffId} Digest: {Digest}", layer.DiffId().Hex.Substring(0,6), layer.Digest().Hex.Substring(0,6));
  }
}
