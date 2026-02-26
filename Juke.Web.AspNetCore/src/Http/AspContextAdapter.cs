using Juke.Web.Core;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspContextAdapter : IHttpContext
{
    private readonly HttpContext _aspNetContext;

    public AspContextAdapter(HttpContext aspNetContext)
    {
        _aspNetContext = aspNetContext;
        Request = new AspRequestAdapter(aspNetContext.Request);
        Response = new AspResponseAdapter(aspNetContext.Response);
        WebSockets = new AspWebSocketManager(aspNetContext.WebSockets); // <-- Инициализация
    }

    public IHttpRequest Request { get; }
    public IHttpResponse Response { get; }
    public IServiceProvider RequestServices => _aspNetContext.RequestServices; 
    public IWebSocketManager WebSockets { get; } // <-- Реализация
}
