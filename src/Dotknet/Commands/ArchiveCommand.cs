using Microsoft.Extensions.Options;
using SharpCompress.Archives.Tar;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using Dotknet.Extensions;

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
  private readonly ILogger<ArchiveCommand> _logger;
  private readonly WriterOptions _tarWriterOptions;

  public ArchiveCommand(IOptions<LifecycleOptions> options, ILogger<ArchiveCommand> logger)
  {
    _options = options.Value;
    _logger = logger;
    _tarWriterOptions = new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None);
  }

  public void Execute()
  {
    using var tarArchive = TarArchive.Create();
    using var memoryStream = new MemoryStream();
    tarArchive.AddAllFromDirectory(_options.DirectoryToArchive!, _options.LayerAppRoot);
    
    tarArchive.SaveTo(memoryStream, _tarWriterOptions);
    var digest = memoryStream.GetDigest();
    var destination = Path.Join(_options.Output!, digest! + ".tar");
    
    using var fileStream = new FileStream(destination!, FileMode.Create, FileAccess.Write);
    memoryStream.WriteTo(fileStream);

    _logger.LogInformation("Layer Digest: {LayerDigest}", "sha256:" + digest);
  }
}
