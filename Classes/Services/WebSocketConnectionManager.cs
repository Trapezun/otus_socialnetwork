using Newtonsoft.Json;
using SocialNetwork.Models;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace SocialNetwork.Classes.Services
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public void AddSocket(WebSocket socket, string socketId)
        {
            _sockets.TryAdd(socketId, socket);
        }

        public void RemoveSocket(string socketId)
        {
            _sockets.TryRemove(socketId, out var socket);
        }

        public async Task ListenSocketAsync(WebSocket socket, string socketId)
        {
            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            RemoveSocket(socketId);
        }

        public async Task SendMessageToAllAsync(string userID, string message)
        {
            if (_sockets.TryGetValue(userID, out WebSocket socket))
            {
                if (socket.State == WebSocketState.Open)
                {
                    var messageBuffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
