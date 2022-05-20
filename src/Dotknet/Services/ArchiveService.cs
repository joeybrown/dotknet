using SharpCompress.Archives.Tar;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Dotknet.Models.Tarball;
using System.IO;
using Dotknet.RegistryClient.Models;

namespace Dotknet.Services;

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
