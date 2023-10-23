using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Net.WebSockets;
using WebSocketDemo.Middleware;

namespace WebSocketDemo.RabbitMQ
{
    public class RabbitMQClient
    {
        private readonly IConnection _conn;
        private readonly IModel _channel;

        public RabbitMQClient()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();

            _channel.ExchangeDeclare(
                exchange: "direct_exchange",
                type: ExchangeType.Direct);
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, e) =>
            {
                await SendMessageToWebSocketClients(e);
            };

            var queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(
                queue: queueName,
                exchange: "direct_exchange",
                routingKey: "web_socket");

            _channel.BasicConsume(
                queue: queueName,
                autoAck: true,
                consumer: consumer);
        }

        private static async Task SendMessageToWebSocketClients(BasicDeliverEventArgs e)
        {
            var clients = WebSocketClientManager.GetAllClients();

            foreach (var client in clients)
            {
                var body = e.Body.ToArray();

                await client.SendAsync(
                    new ArraySegment<byte>(body, 0, body.Length),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None);
            }
        }
    }
}
