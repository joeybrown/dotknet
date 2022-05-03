using Microsoft.Extensions.Options;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives;
using SharpCompress.Writers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Dotknet.Services;

namespace Dotknet.Commands;

public interface IArchiveCommand
{
  void Execute();
}

/// It would be cool if dotnet had native APIs for this.
/// https://github.com/dotnet/runtime/issues/65951
public class ArchiveCommand : IArchiveCommand
{
  private readonly LifecycleOptions _options;
  private readonly IDigestService _digestService;
  private readonly ILogger<ArchiveCommand> _logger;
  private readonly WriterOptions _tarWriterOptions;

  public ArchiveCommand(IOptions<LifecycleOptions> options, ILogger<ArchiveCommand> logger, IDigestService digestService)
  {
    _options = options.Value;
    _digestService = digestService;
    _logger = logger;
    _tarWriterOptions = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None);
  }

  public void Execute()
  {
    // todo: clean this up
    using var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory(_options.DirectoryToArchive!, _options.LayerAppRoot);
    
    using var memoryStream = new MemoryStream();
    tarArchive.SaveTo(memoryStream, _tarWriterOptions);
    
    var memoryDigest = _digestService.GetDigest(memoryStream);

    var destination = Path.Join(_options.Output!, memoryDigest! + ".tar");
    var fileStream = new FileStream(destination!, FileMode.Create, FileAccess.Write);
    memoryStream.WriteTo(fileStream);
    fileStream.Close();

    var file = File.OpenRead(destination!);
    var fileDigest = _digestService.GetDigest(file);
    file.Close();

    var newDestination = Path.Join(_options.Output!, fileDigest! + ".tar");

    System.IO.File.Move(destination, newDestination);

    _logger.LogInformation("Layer Digest: {LayerDigest}", "sha256:" + memoryDigest);
    _logger.LogInformation("From File Digest {FileDigest}", "sha256:" + fileDigest);
    
  }
}

public static class IWritableArchiveExtensions
{
  public static void AddAllFromDirectory(this IWritableArchive writableArchive, string filePath, string rootDir, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
  {
    using (writableArchive.PauseEntryRebuilding())
    {
      foreach (var path in Directory.EnumerateFiles(filePath, searchPattern, searchOption))
      {
        var fileInfo = new FileInfo(path);
        var key = rootDir + "/" + path.Substring(filePath.Length);
        writableArchive.AddEntry(key, fileInfo.OpenRead(), true, fileInfo.Length, fileInfo.LastWriteTime);
      }
    }
  }
}
