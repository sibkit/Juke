using Juke.Web.Core;
using Microsoft.AspNetCore.Http;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace Juke.Web.AspNetCore;

public class AspNetCoreContextAdapter : IHttpContext
{
    public AspNetCoreContextAdapter(HttpContext aspNetContext)
    {
        Request = new AspNetCoreRequestAdapter(aspNetContext.Request);
        Response = new AspNetCoreResponseAdapter(aspNetContext.Response);
    }

    public IHttpRequest Request { get; }
    public IHttpResponse Response { get; }
}

public class AspNetCoreRequestAdapter : IHttpRequest
{
    private readonly HttpRequest _request;
    private Method _method;
    public AspNetCoreRequestAdapter(HttpRequest request) => _request = request;

    Method IHttpRequest.Method {
        get => _method;
        init => _method = value;
    }

    string IHttpRequest.Path { get; init; }
    public string QueryString { get; }
    public Dictionary<string, string> RouteValues { get; }
    public IReadOnlyDictionary<string, string> Headers { get; }
    public Stream Body { get; }
    public string Path => _request.Path.Value ?? "/";
    public string Method => _request.Method;
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

    public IDictionary<string, string> Headers { get; }
    public Stream Body { get; }

    public Task WriteAsync(string content)
    {
        // Пробрасываем асинхронный вызов дальше в ASP.NET Core
        return _response.WriteAsync(content); 
    }
}