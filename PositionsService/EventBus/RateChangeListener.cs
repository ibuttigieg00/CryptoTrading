using Newtonsoft.Json;
using PositionsService.Models;
using PositionsService.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PositionsService.EventBus
{
    public class RateChangeListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName = "rateChangeQueue";
        private readonly string _hostname;
        private readonly int _Port;

        // Constructor to initialize the listener with position service and RabbitMQ configuration
        public RateChangeListener(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQSettings:HostName"]!;
            _Port = Convert.ToInt32(configuration["RabbitMQSettings:Port"]!);

            var factory = new ConnectionFactory() { HostName = _hostname, Port = _Port };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue to listen to
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void StartListening(PositionService positionService)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var rateChange = JsonConvert.DeserializeObject<RateChangeMessage>(message);

                    // Update the position based on the rate change
                    string result = (positionService.CalculateProfitLoss(rateChange.Symbol, rateChange.NewRate));

                    if (!string.IsNullOrEmpty(result))
                    {
                        Console.WriteLine(result);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Error deserializing message: {jsonEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing the rate change message: {ex.Message}");
                }
            };

            // Start consuming messages
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        // Cleanup on close
        public void Close()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}