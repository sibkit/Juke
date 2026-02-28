using Juke.ServiceLocation;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspContextAdapter : IHttpContext
{
    private readonly HttpContext _aspNetContext;

    public AspContextAdapter(HttpContext aspNetContext, IScope scope)
    {
        _aspNetContext = aspNetContext;
        Request = new AspRequestAdapter(aspNetContext.Request);
        Response = new AspResponseAdapter(aspNetContext.Response);
        WebSockets = new AspWebSocketManager(aspNetContext.WebSockets);
        
        // Теперь контекст отдает наш собственный Scope
        RequestServices = scope; 
    }

    public IHttpRequest Request { get; }
    public IHttpResponse Response { get; }
    public IScope RequestServices { get; } 
    public IWebSocketManager WebSockets { get; }
}