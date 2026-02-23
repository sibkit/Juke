using System.Text;

namespace Juke.Web.Core;

public enum Method {
    UNDEFINED = -1,
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3,
    PATCH = 4,   
    OPTIONS = 5, 
    HEAD = 6
}

public interface IHttpContext {
    IHttpRequest Request { get; }
    IHttpResponse Response { get; }
    IServiceProvider RequestServices { get; } 
}

public interface IHttpRequest {
    Method Method { get; }
    string Path { get; } // Removed init, interfaces should define getters for adapters
    string QueryString { get; } 
    
    Dictionary<string, object> RouteValues { get; }
    Stream Body { get; }

    // Direct method is faster and safer than allocating dictionaries
    string? GetHeader(string key); 
}

public interface IHttpResponse {
    int StatusCode { get; set; }
    Stream Body { get; }
    
    void AddHeader(string key, string value); 
    string? GetHeader(string key); // <-- Добавили метод для чтения заголовков
}

public static class HttpResponseExtensions
{
    public static void SetContentType(this IHttpResponse response, string contentType) {
        response.AddHeader("Content-Type", contentType);
    }

    public static string? GetContentType(this IHttpResponse response) {
        return response.GetHeader("Content-Type");
    }
    
    public static async Task WriteAsync(this IHttpResponse response, string content) {
        var bytes = Encoding.UTF8.GetBytes(content);
        await response.Body.WriteAsync(bytes.AsMemory()); 
    }
}

public static class ServiceProviderExtensions {
    public static T GetRequiredService<T>(this IServiceProvider provider) where T : notnull {
        var service = provider.GetService(typeof(T));
        if (service == null) {
            throw new InvalidOperationException($"Service '{typeof(T).Name}' is not registered.");
        }
        return (T)service;
    }
}