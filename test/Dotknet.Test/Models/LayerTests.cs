using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using SharpCompress.Archives.Tar;
using Xunit;

namespace Dotknet.Test.Models;

public class LayerTests
{
  [Fact]
  public void LayerModifiedTests()
  {
    var archive = TarArchive.Create();
    var timestamp = new DateTime(1990, 05, 13, 0, 0, 0);
    using var text = GenerateStreamFromString("foo foo foo foo foo foo foo");
    archive.AddEntry("bar", text, text.Length, timestamp);
    archive.AddEntry("baz", text, text.Length, timestamp);
    using var layer = new TarballLayer(archive);
    using var compressed = layer.Compressed();
    compressed.Seek(0, SeekOrigin.Begin);
    using var ms2 = new MemoryStream();
    compressed.CopyTo(ms2);
    ms2.Seek(0, SeekOrigin.Begin);
    var bytes = ms2.ToArray();
    var timestampBytes = bytes.Skip(4).Take(4).ToArray();
    var seconds = BitConverter.ToUInt32(timestampBytes, 0);
    var EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var datetime = EPOCH.AddSeconds(seconds);
    datetime.Should().Be(new DateTime(1980, 01, 01, 00, 00, 01, DateTimeKind.Utc));
  }

  public static Stream GenerateStreamFromString(string s)
  {
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write(s);
    writer.Flush();
    stream.Position = 0;
    return stream;
  }
}
