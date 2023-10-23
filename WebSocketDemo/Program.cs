using WebSocketDemo.RabbitMQ;

namespace WebSocketDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rabbitMQClient = new RabbitMQClient();
            rabbitMQClient.StartListening();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}