using Microsoft.Extensions.Options;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives;
using SharpCompress.Writers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

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
  private readonly WriterOptions _tarWriterOptions;
  private readonly ILogger<ArchiveCommand> _logger;

  public ArchiveCommand(IOptions<LifecycleOptions> options, ILogger<ArchiveCommand> logger)
  {
    _options = options.Value;
    _tarWriterOptions = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None);
    _logger = logger;
  }

  public void Execute()
  {
    using var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory(_options.DirectoryToArchive!, _options.LayerAppRoot);

    using var memoryStream = new MemoryStream();
    tarArchive.SaveTo(memoryStream, _tarWriterOptions);

    using var sha256Hash = SHA256.Create();
    var layerDigest = GetHash(sha256Hash, memoryStream);
    var destination = Path.Join(_options.Output!, layerDigest! + ".tar");
    
    using var fileStream = new FileStream(destination!, FileMode.Create, FileAccess.Write);
    memoryStream.WriteTo(fileStream);
    _logger.LogInformation("Layer Digest: {LayerDigest}", "sha256:" + layerDigest);
  }

  private static string GetHash(HashAlgorithm hashAlgorithm, MemoryStream input)
  {
    var data = hashAlgorithm.ComputeHash(input.GetBuffer());
    var sBuilder = new StringBuilder();
    for (int i = 0; i < data.Length; i++)
    {
      sBuilder.Append(data[i].ToString("x2"));
    }
    return sBuilder.ToString();
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
