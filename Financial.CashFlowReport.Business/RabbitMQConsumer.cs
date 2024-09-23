using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class RabbitMQConsumer : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMQConsumer(IConnectionFactory connectionFactory, ILogger<RabbitMQConsumer> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();

        _logger.LogInformation("RabbitMQ connection established.");
    }

    public void StartConsuming(string queueName, Action<string> onMessageReceived)
    {
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Received message: {message}", message);
            onMessageReceived(message);
        };

        // Iniciar o consumo e manter o canal ativo
        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        _logger.LogInformation($"Consuming messages from queue: {queueName}");
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _logger.LogInformation("RabbitMQ connection and channel closed.");
    }
}
