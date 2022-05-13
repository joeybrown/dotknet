using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace Dotknet.RegistryClient;

public static class ServiceCollectionExtensions{
  public static IServiceCollection AddRegistryClientServices(this IServiceCollection services, Action<RegistryClientConfiguration> configurator) {
    services.Configure<RegistryClientConfiguration>(configurator);
    services.AddHttpClient<IRegistryClient, RegistryClient>((provider, client) => { 
      var config = provider.GetRequiredService<IOptions<RegistryClientConfiguration>>().Value;
      client.Timeout = config.Timeout;
    });
    services.AddSingleton<IRegistryClientFactory, RegistryClientFactory>();
    return services;
  }
}
