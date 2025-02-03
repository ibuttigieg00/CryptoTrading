using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;

namespace RatesService.EventBus
{
    public interface IEventBus
    {
        void PublishRateChange(string symbol, decimal newRate);
        void Close();
    }

    public class RabbitMQProducer : IEventBus
    {
        private readonly string _hostname;
        private readonly int _Port;
        private readonly string _queueName = "rateChangeQueue";
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQSettings:HostName"]!;
            _Port = Convert.ToInt32(configuration["RabbitMQSettings:Port"]!);

            // Create a connection factory
            var factory = new ConnectionFactory() { HostName = _hostname, Port = _Port };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void PublishRateChange(string symbol, decimal newRate)
        {
            var rateChange = new { Symbol = symbol, NewRate = newRate };

            // Convert the rate change object to JSON
            var message = JsonConvert.SerializeObject(rateChange);
            var body = Encoding.UTF8.GetBytes(message);

            // Publish the message to RabbitMQ
            _channel.BasicPublish(exchange: "",
                                 routingKey: _queueName,
                                 basicProperties: null,
                                 body: body);

            Console.WriteLine($" [x] Sent '{message}'");
        }

        public void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}