
/* Juke.Web.Core/WebSockets/AppWebSocketHub.cs */
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;


namespace Juke.Web.Core.WebSockets;

public class AppWebSocketHub
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();

    public void AddClient(Guid id, WebSocket socket) => _clients.TryAdd(id, socket);
    public void RemoveClient(Guid id) => _clients.TryRemove(id, out _);

    // Главный метод маршрутизации сообщений по каналам!
    public async Task BroadcastAsync(string channel, object payload)
    {
        var envelope = new { c = channel, p = payload };
        var json = JsonSerializer.Serialize(envelope);
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);

        foreach (var pair in _clients)
        {
            if (pair.Value.State == WebSocketState.Open)
            {
                try {
                    await pair.Value.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
                } catch {
                    // Игнорируем ошибки при отправке, если сокет внезапно закрылся
                }
            }
        }
    }
}