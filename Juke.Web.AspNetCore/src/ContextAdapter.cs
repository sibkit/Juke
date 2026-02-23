using Juke.Web.Core;
using Microsoft.AspNetCore.Http;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Juke.Web.AspNetCore;

public class ContextAdapter : IHttpContext
{
    private readonly HttpContext _aspNetContext;

    public ContextAdapter(HttpContext aspNetContext)
    {
        _aspNetContext = aspNetContext;
        Request = new AspNetCoreRequestAdapter(aspNetContext.Request);
        Response = new AspNetCoreResponseAdapter(aspNetContext.Response);
    }

    public IHttpRequest Request { get; }
    public IHttpResponse Response { get; }
    
    public IServiceProvider RequestServices => _aspNetContext.RequestServices; 
}

public class AspNetCoreRequestAdapter : IHttpRequest
{
    private readonly HttpRequest _request;

    public AspNetCoreRequestAdapter(HttpRequest request) 
    {
        _request = request;
        
        // Parse HTTP Method securely
        if (Enum.TryParse<Method>(request.Method, ignoreCase: true, out var parsedMethod)) {
            Method = parsedMethod;
        } else {
            Method = Juke.Web.Core.Method.UNDEFINED;
        }

        // Must be initialized, as Router writes into it!
        RouteValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase); 
    }

    public Method Method { get; }

    // Mapping properties directly to avoid NullReferenceException
    public string Path => _request.Path.Value ?? "/";
    public string QueryString => _request.QueryString.Value ?? string.Empty;
    public Dictionary<string, object> RouteValues { get; }
    public Stream Body => _request.Body;

    // Fast mapping to ASP.NET Core IHeaderDictionary
    public string? GetHeader(string key) => _request.Headers[key];
}

public class AspNetCoreResponseAdapter : IHttpResponse
{
    private readonly HttpResponse _response;

    public AspNetCoreResponseAdapter(HttpResponse response) => _response = response;

    public int StatusCode 
    { 
        get => _response.StatusCode; 
        set => _response.StatusCode = value; 
    }

    public Stream Body => _response.Body;

    public void AddHeader(string key, string value)
    {
        _response.Headers[key] = value; 
    }

    // <-- Реализация метода чтения
    public string? GetHeader(string key)
    {
        if (_response.Headers.TryGetValue(key, out var values)) {
            return values.ToString();
        }
        return null;
    }
}