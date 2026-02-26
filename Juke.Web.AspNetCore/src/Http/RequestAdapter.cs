using Juke.Web.Core;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspRequestAdapter : IHttpRequest
{
    private readonly HttpRequest _request;

    public AspRequestAdapter(HttpRequest request) 
    {
        _request = request;
        
        // Parse HTTP Method securely
        Method = Enum.TryParse<Method>(request.Method, ignoreCase: true, out var parsedMethod) ? 
            parsedMethod : Method.UNDEFINED;

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