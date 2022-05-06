using SharpCompress.Archives;

public static class IWritableArchiveExtensions
{
  // timestamp necessary for reproducability
  public static void AddAllFromDirectory(this IWritableArchive writableArchive, string filePath, string rootDir, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories, DateTime? timestamp = null)
  {
    if (!timestamp.HasValue) {
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