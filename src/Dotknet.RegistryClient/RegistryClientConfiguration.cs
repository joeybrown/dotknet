using System;

namespace Dotknet.RegistryClient;

public class RegistryClientConfiguration
{
  public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(1);
}
