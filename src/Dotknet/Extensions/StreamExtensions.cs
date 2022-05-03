using System.Security.Cryptography;
using System.Text;

namespace Dotknet.Extensions;

public static class StreamExtensions {

  public static string GetDigest(this Stream stream)
  {
    using var hashAlgorithm = SHA256.Create();
    stream.Seek(0, SeekOrigin.Begin);
    var data = hashAlgorithm.ComputeHash(stream);
    var sBuilder = new StringBuilder();
    for (int i = 0; i < data.Length; i++)
    {
      sBuilder.Append(data[i].ToString("x2"));
    }
    return sBuilder.ToString();
  }
}