using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitStudies.Contracts;
using RabbitStudies.RabbitMq;

namespace RabbitStudies.Consumers;

public class ConsumerA : RabbitMqConsumerBase
{
    private readonly ILogger<ConsumerA> _logger;
    private readonly IConnection _rabbitConnection;
    private readonly IModel _channel;

    private const string _queueName = "QUEUE_A";

    public ConsumerA(
        ILogger<ConsumerA> logger,
        RabbitMqConnection rabbitConnection)
        : base(rabbitConnection, logger, _queueName, 1000)
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection.Connection;
        _channel = _rabbitConnection.CreateModel();
    }

    protected override Task<bool> HandleMessage(
        BasicDeliverEventArgs rabbitEventArgs,
        RabbitMessage message)
    {
        HelloWorldMessage? helloWorldMessage = message
            .GetDeserializedMessage<HelloWorldMessage>();

        if (helloWorldMessage is null)
        {
            _logger.LogError("Error while deserializing rabbit message");
            return Task.FromResult(false);
        }

        _logger.LogInformation("Message Received at {ConsumerName} - Id: {Id}",
            nameof(ConsumerA),
            helloWorldMessage.Id);

        return Task.FromResult(true);
    }
}
