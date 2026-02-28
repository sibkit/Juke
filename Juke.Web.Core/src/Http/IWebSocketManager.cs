using System.Net.WebSockets;

namespace Juke.Web.Core.Http;

public interface IWebSocketManager 
{
    bool IsWebSocketRequest { get; }
    Task<WebSocket> AcceptAsync();
}