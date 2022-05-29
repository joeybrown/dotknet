using System.IO;
using System.Security.Cryptography;
using System.Text;
using Dotknet.RegistryClient.Models;

namespace Dotknet.RegistryClient.Extensions;

public static class StreamExtensions {

  public static Hash GetHash(this Stream stream)
  {
    using var hashAlgorithm = SHA256.Create();
    stream.Seek(0, SeekOrigin.Begin);
    var data = hashAlgorithm.ComputeHash(stream);
    var sBuilder = new StringBuilder();
    for (int i = 0; i < data.Length; i++)
    {
      sBuilder.Append(data[i].ToString("x2"));
    }
    
    return new Hash {
      Hex = sBuilder.ToString(),
      Algorithm = "sha256"
    };
  }
}
