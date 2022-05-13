using System;
using Microsoft.Extensions.DependencyInjection;

namespace Dotknet.RegistryClient;

public interface IRegistryClientFactory
{
  IRegistryClient Create();
}

public class RegistryClientFactory : IRegistryClientFactory
{
  private readonly IServiceProvider _serviceProvider;

  public RegistryClientFactory(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public IRegistryClient Create()
  {
    return _serviceProvider.GetRequiredService<IRegistryClient>();
  }
}
