using Juke.ServiceLocation;

namespace Juke.Web.Core.Http;

public interface IHttpContext {
    IHttpRequest Request { get; }
    IHttpResponse Response { get; }
    IScope RequestServices { get; } 
    IWebSocketManager WebSockets { get; }
}