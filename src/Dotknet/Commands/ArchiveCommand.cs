using Microsoft.Extensions.Options;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives;

namespace Dotknet.Commands;

public interface IArchiveCommand
{
  void Execute();
}

/// https://github.com/dotnet/runtime/issues/65951
public class ArchiveCommand : IArchiveCommand
{
  private readonly LifecycleOptions _options;

  public ArchiveCommand(IOptions<LifecycleOptions> options)
  {
    _options = options.Value;
  }

  public void Execute()
  {
    using var tarArchive = TarArchive.Create();
    tarArchive.AddAllFromDirectory(_options.DirectoryToArchive!, _options.LayerAppRoot);
    var writerOptions = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None);
    tarArchive.SaveTo(_options.ArchiveOutput!, writerOptions);
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
