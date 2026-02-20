using System.Text;

namespace Juke.Web.Core;

public enum Method {
    UNDEFINED = -1,
    GET = 0,
    POST = 1,
    PUT = 2,
    DELETE = 3
}

public interface IHttpRequest {
    Method Method { get; init; }
    string Path { get; init; }
    string QueryString { get; } 
    
    Dictionary<string, string> RouteValues { get; }
    IReadOnlyDictionary<string, string> Headers { get; }
    
    Stream Body { get; }
}

public interface IHttpResponse {
    int StatusCode { get; set; }
    IDictionary<string, string> Headers { get; }
    Stream Body { get; }
}

public interface IHttpContext {
    IHttpRequest Request { get; }
    IHttpResponse Response { get; }
}

public static class HttpResponseExtensions
{
    public static void SetContentType(this IHttpResponse response, string contentType) {
        response.Headers["Content-Type"] = contentType;
    }

    public static string? GetContentType(this IHttpResponse response) {
        return response.Headers.TryGetValue("Content-Type", out var type) ? type : null;
    }
    
    public static async Task WriteAsync(this IHttpResponse response, string content) {
        var bytes = Encoding.UTF8.GetBytes(content);
        await response.Body.WriteAsync(bytes, 0, bytes.Length);
    }
    
    // Сюда же в будущем отлично ляжет метод WriteJsonAsync<T>(...)
    // public static async Task WriteJsonAsync<T>(this IHttpResponse response, T data) { ... }
}