using System.Net.WebSockets;

namespace WebSocketDemo.Middleware
{
    public class WebSocketServer
    {
        private readonly RequestDelegate _next;

        public WebSocketServer(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await HandleWebSocket(socket);
        }

        private static async Task HandleWebSocket(WebSocket socket)
        {
            WebSocketClientManager.AddClient(socket);

            while (socket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
                var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
            }

            WebSocketClientManager.RemoveClient(socket);
        }
    }
}
