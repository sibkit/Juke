using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using Juke.Web.Core.Http;
using Juke.Web.Core.WebSockets;

namespace Juke.Web.Core.Handlers;

public class AppWebSocketHandler : WebSocketHandler
{
    private static readonly ConditionalWeakTable<WebSocket, BoxedGuid> _socketIds = new();

    protected override Task OnConnectedAsync(WebSocket socket, IHttpContext context)
    {
        var hub = context.RequestServices.Get<AppWebSocketHub>();
        var id = Guid.NewGuid();
        _socketIds.Add(socket, new BoxedGuid(id));
        
        hub.AddClient(id, socket);
        return Task.CompletedTask;
    }

    protected override Task OnDisconnectedAsync(WebSocket socket, IHttpContext context)
    {
        var hub = context.RequestServices.Get<AppWebSocketHub>();
        if (_socketIds.TryGetValue(socket, out var id)) {
            hub.RemoveClient(id.Value);
        }
        return Task.CompletedTask;
    }

    private class BoxedGuid { public readonly Guid Value; public BoxedGuid(Guid v) => Value = v; }
}