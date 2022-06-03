using SharpCompress.Archives.Tar;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.IO;
using Dotknet.RegistryClient.Models;
using SharpCompress.Archives;
using System;

namespace Dotknet;

public interface IArchiveService
{
  Task<ILayer> Execute(string source, string layerRoot);
}

/// It would be cool if dotnet had native APIs for this.
/// https://github.com/dotnet/runtime/issues/65951
public class ArchiveService : IArchiveService
{
  private readonly ILogger<ArchiveService> _logger;
  private readonly WriterOptions _uncompressed;
  private readonly WriterOptions _compressed;

  public ArchiveService(ILogger<ArchiveService> logger)
  {
    _logger = logger;

  }

  public Task<ILayer> Execute(string source, string layerRoot)
  {
    var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory((new FileInfo(source)).FullName, layerRoot);
    ILayer layer = new TarballLayer(tarArchive);
    return Task.FromResult(layer);
  }
}

public static class IWritableArchiveExtensions
{
  // timestamp necessary for reproducibility
  public static void AddAllFromDirectory(this IWritableArchive writableArchive, string filePath, string rootDir, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories, DateTime? timestamp = null)
  {
    if (!timestamp.HasValue)
    {
      timestamp = new DateTime(1980, 01, 01, 00, 00, 01);
    }
    using (writableArchive.PauseEntryRebuilding())
    {
      foreach (var path in Directory.EnumerateFiles(filePath, searchPattern, searchOption))
      {
        var fileInfo = new FileInfo(path);
        var key = rootDir + path.Substring(filePath.Length);
        writableArchive.AddEntry(key, fileInfo.OpenRead(), true, fileInfo.Length, timestamp);
      }
    }
  }
}
