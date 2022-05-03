using System.IO;
using Dotknet.Services;
using FluentAssertions;
using Xunit;

public class ManifestTests
{
  [Fact]
  public void Digest_Should_Match_sha256_Cli()
  {
    var sourceFile = "TestResources/layer.tar";
    var file = File.OpenRead(sourceFile);

    var sut = new DigestService();
    var actual = sut.GetDigest(file);

    // expected value was obtained by cli `sha256sum TestResources/layer.tar`
    const string expected = "e9f4aefa0a1c6516dc584402aedf90529c69a5bf4841dbfc6c2309b046e3d048";

    actual.Should().BeEquivalentTo(expected);
  }
}