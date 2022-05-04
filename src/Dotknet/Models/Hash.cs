namespace Dotknet.Models;

/// ref: https://github.com/google/go-containerregistry/blob/main/pkg/v1/hash.go
public class Hash
{
  // Algorithm holds the algorithm used to compute the hash.
  public string Algorithm { get; set; }

  // Hex holds the hex portion of the content hash.
  public string Hex { get; set; }

  public override string ToString()
  {
    return $"{Algorithm}:{Hex}";
  }
}