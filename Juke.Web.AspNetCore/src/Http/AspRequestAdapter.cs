using Juke.Web.Core;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspRequestAdapter : IHttpRequest {
    private readonly HttpRequest _request;

    public AspRequestAdapter(HttpRequest request) 
    {
        _request = request;
        Method = Enum.TryParse<Method>(request.Method, ignoreCase: true, out var parsedMethod) ? 
            parsedMethod : Method.UNDEFINED;
        RouteValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); 
    }

    public Method Method { get; }
    public string Path => _request.Path.Value ?? "/";
    public string QueryString => _request.QueryString.Value ?? string.Empty;
    public Dictionary<string, object> RouteValues { get; }
    public Stream Body => _request.Body;
    public string? GetHeader(string key) => _request.Headers[key];

    // --- РЕАЛИЗАЦИЯ ФАЙЛОВ ---
    public bool HasFormContentType => _request.HasFormContentType;
    
    // Лениво читаем файлы только если это форма. Zero-allocation для обычных GET запросов!
    public IEnumerable<IUploadedFile> Files => _request.HasFormContentType 
        ? _request.Form.Files.Select(f => new AspUploadedFile(f)) 
        : Array.Empty<IUploadedFile>();
}