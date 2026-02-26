using System.Net.WebSockets;
using Juke.Web.Core;
using Juke.Web.Core.Http;
using Microsoft.AspNetCore.Http;

namespace Juke.Web.AspNetCore.Http;

public class AspWebSocketManager : IWebSocketManager
{
    private readonly WebSocketManager _manager;
    public AspWebSocketManager(WebSocketManager manager) => _manager = manager;

    public bool IsWebSocketRequest => _manager.IsWebSocketRequest;
    public Task<WebSocket> AcceptAsync() => _manager.AcceptWebSocketAsync();
}