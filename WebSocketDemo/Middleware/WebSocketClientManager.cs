using System.Net.WebSockets;

namespace WebSocketDemo.Middleware
{
    public static class WebSocketClientManager
    {
        private static readonly List<WebSocket> _clients = new();

        public static void AddClient(WebSocket client)
        {
            _clients.Add(client);
        }

        public static void RemoveClient(WebSocket client)
        {
            _clients.Remove(client);
        }

        public static List<WebSocket> GetAllClients()
        {
            return _clients;
        }
    }
}
