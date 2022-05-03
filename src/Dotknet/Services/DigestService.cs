using System.Security.Cryptography;
using System.Text;

namespace Dotknet.Services;

public interface IDigestService
{
  string GetDigest(Stream stream);
}

public class DigestService : IDigestService
{
  public string GetDigest(Stream stream)
  {
    using var sha256Hash = SHA256.Create();
    return GetHash(sha256Hash, stream);
  }

  private static string GetHash(HashAlgorithm hashAlgorithm, Stream input)
  {
    var data = hashAlgorithm.ComputeHash(input);
    var sBuilder = new StringBuilder();
    for (int i = 0; i < data.Length; i++)
    {
      sBuilder.Append(data[i].ToString("x2"));
    }
    return sBuilder.ToString();
  }
}