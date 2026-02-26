/* WebSocketHandler.cs */
using System.Net.WebSockets;
using Juke.Web.Core.Http;

namespace Juke.Web.Core.Handlers;

public abstract class WebSocketHandler : IRequestHandler
{
    public async Task HandleAsync(IHttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptAsync();
        await OnConnectedAsync(webSocket, context);

        var buffer = new byte[1024 * 4];
        
        try 
        {
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            while (!receiveResult.CloseStatus.HasValue)
            {
                await OnMessageReceivedAsync(webSocket, receiveResult, buffer, context);
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            
            await OnDisconnectedAsync(webSocket, context);
            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
        }
        catch (WebSocketException) 
        {
            await OnDisconnectedAsync(webSocket, context);
        }
    }

    protected virtual Task OnConnectedAsync(WebSocket socket, IHttpContext context) => Task.CompletedTask;
    protected virtual Task OnDisconnectedAsync(WebSocket socket, IHttpContext context) => Task.CompletedTask;
    protected virtual Task OnMessageReceivedAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer, IHttpContext context) => Task.CompletedTask;
}