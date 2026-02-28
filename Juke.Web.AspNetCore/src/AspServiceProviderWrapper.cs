using Juke.ServiceLocation;

namespace Juke.Web.AspNetCore;

public class AspServiceProviderWrapper : IServiceLocator {
    private readonly IServiceProvider _provider;
    
    public AspServiceProviderWrapper(IServiceProvider provider) => _provider = provider;
    
    public T Get<T>() {
        var service = _provider.GetService(typeof(T));
        if (service == null) throw new InvalidOperationException($"Service {typeof(T).Name} not found in ASP.NET Core Fallback DI.");
        return (T)service;
    }
}