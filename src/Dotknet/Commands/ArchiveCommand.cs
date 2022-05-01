using ICSharpCode.SharpZipLib.Tar;
using Microsoft.Extensions.Options;
using System.IO.Compression;

namespace Dotknet.Commands;

public interface IArchiveCommand
{
  void Execute();
}

///
/// https://github.com/dotnet/runtime/issues/65951
///
public class ArchiveCommand : IArchiveCommand
{
  private readonly LifecycleOptions _options;

  public ArchiveCommand(IOptions<LifecycleOptions> options)
  {
    _options = options.Value;
  }

  public void Execute()
  {
    using var archiveStream = new MemoryStream();
    using var tarArchive = archiveStream.CreateDirectoryArchive(_options.DirectoryToArchive!);
    using var fileStream = new FileStream(_options.ArchiveOutput!, FileMode.Create, FileAccess.Write);
    archiveStream.WriteTo(fileStream);
  }
}

public static class MemoryStreamExtensions
{
  /// ref: https://stackoverflow.com/questions/31836519/how-to-create-tar-gz-file-in-c-sharp
  public static TarArchive CreateDirectoryArchive(this MemoryStream memoryStream, string sourceDirectory)
  {
    var tarArchive = TarArchive.CreateOutputTarArchive(memoryStream);
    tarArchive.RootPath = "/dotknet-app";
    AddDirectoryFilesToTGZ(tarArchive, sourceDirectory);
    return tarArchive;
  }

  private static void AddDirectoryFilesToTGZ(TarArchive tarArchive, string sourceDirectory)
  {
    AddDirectoryFilesToTGZ(tarArchive, sourceDirectory, string.Empty);
  }

  private static void AddDirectoryFilesToTGZ(TarArchive tarArchive, string sourceDirectory, string currentDirectory)
  {
    var pathToCurrentDirectory = Path.Combine(sourceDirectory, currentDirectory);

    // Write each file to the tgz.
    var filePaths = Directory.GetFiles(pathToCurrentDirectory);
    foreach (string filePath in filePaths)
    {
      var tarEntry = TarEntry.CreateEntryFromFile(filePath);

      // Name sets where the file is written. Write it in the same spot it exists in the source directory
      tarEntry.Name = filePath.Replace(sourceDirectory, "");

      // If the Name starts with '\' then an extra folder (with a blank name) will be created, we don't want that.
      if (tarEntry.Name.StartsWith('\\'))
      {
        tarEntry.Name = tarEntry.Name.Substring(1);
      }
      tarArchive.WriteEntry(tarEntry, true);
    }

    // Write directories to tgz
    var directories = Directory.GetDirectories(pathToCurrentDirectory);
    foreach (string directory in directories)
    {
      AddDirectoryFilesToTGZ(tarArchive, sourceDirectory, directory);
    }
  }
}
