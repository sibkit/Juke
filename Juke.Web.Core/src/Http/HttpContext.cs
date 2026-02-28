using System.Net.WebSockets;
using System.Text;

namespace Juke.Web.Core.Http;











public static class ServiceProviderExtensions {
    public static T Get<T>(this IServiceProvider provider) where T : notnull {
        var service = provider.GetService(typeof(T));
        if (service == null) {
            throw new InvalidOperationException($"Service '{typeof(T).Name}' is not registered.");
        }
        return (T)service;
    }
}