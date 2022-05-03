using SharpCompress.Archives;

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