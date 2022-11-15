using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace RabbitStudies.RabbitMq;

public abstract class RabbitMqConsumerBase : BackgroundService
{
    private readonly ILogger<RabbitMqConsumerBase> _logger;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly ushort _prefetchCount;
    private readonly uint _prefetchSize = 0;

    protected RabbitMqConsumerBase(
        RabbitMqConnection rabbitConnection,
        ILogger<RabbitMqConsumerBase> logger,
        string queueName,
        ushort prefetchCount = 1)
    {
        ArgumentNullException.ThrowIfNull(queueName, nameof(queueName));

        _logger = logger;
        _queueName = queueName;
        _prefetchCount = prefetchCount;

        var connection = rabbitConnection.Connection;

        _channel = connection.CreateModel();
    }

    protected uint PrefetchSize { get; set; } = 0;
    protected ushort PrefetchCount { get; set; } = 1;
    protected int MaxRetryCount { get; set; } = 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Consuming from [{QueueName}]", _queueName);

        DeclareQueue();

        StartConsumer();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }

        _logger.LogDebug("Finish consuming [{QueueName}]", _queueName);
    }

    private void DeclareQueue()
    {
        _logger.LogInformation("Declaring queue {queueName}", _queueName);

        _ = _channel.QueueDeclare(
            _queueName,
            true,
            false,
            false);
    }

    private void StartConsumer()
    {
        _logger.LogInformation("Starting consumer for queue {queueName}", _queueName);

        _channel.BasicQos(_prefetchSize, _prefetchCount, false);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += MessageReceived;

        _channel.BasicConsume(_queueName, false, consumer);
    }

    private async Task MessageReceived(object sender, BasicDeliverEventArgs rabbitEventArgs)
    {
        try
        {
            RabbitMessage? message = rabbitEventArgs.GetRabbitMessage();

            if (message is null)
            {
                _logger.LogError("Error while deserializing rabbit message");
                return;
            }

            _logger.LogInformation("{MessageType} received on {ConsumerType}", message.MessageType.ToString(), GetType().Name);

            var result = await HandleMessage(rabbitEventArgs, message);

            if (!result)
            {
                _logger.LogError("Error processing message");
                return;
            }

            _channel.BasicAck(rabbitEventArgs.DeliveryTag, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception captured");
        }
    }

    protected abstract Task<bool> HandleMessage(
        BasicDeliverEventArgs rabbitEventArgs,
        RabbitMessage message);
}
