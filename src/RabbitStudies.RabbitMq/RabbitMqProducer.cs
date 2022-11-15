using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitStudies.RabbitMq;
public class RabbitMqProducer
{
    private readonly RabbitMqConnection _rabbitConnection;
    private readonly ILogger<RabbitMqProducer> _logger;

    private readonly object _locker = new();

    public RabbitMqProducer(RabbitMqConnection rabbitConnection, ILogger<RabbitMqProducer> logger)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;

        _channel = ObterChannel();
    }

    private IConnection Conexao => _rabbitConnection.Connection;

    private IModel _channel;
    private IModel Channel => ObterChannel();

    private IModel ObterChannel()
    {
        if (_channel is not { IsOpen: true })
        {
            lock (_locker)
            {
                if (_channel is not { IsOpen: true })
                {
                    _channel?.Dispose();
                    _channel = Conexao.CreateModel();
                    return _channel;
                }
            }
        }

        return _channel;
    }

    public bool Publish<TMessage>
        (TMessage message, MessageType messageType, string routingKey)
    {
        var serializedMessage = JsonConvert.SerializeObject(message);

        var messageBase = new RabbitMessage(serializedMessage, messageType);

        return PublishMessage(JsonConvert.SerializeObject(messageBase), routingKey);
    }

    private bool PublishMessage(string serializedMessage, string routingKey)
    {
        try
        {
            var mensagemBytes = Encoding.UTF8.GetBytes(serializedMessage);

            var propriedades = Channel.CreateBasicProperties();

            propriedades.Persistent = true;
            propriedades.CreateRetryCountHeader();

            _channel.BasicPublish("",
                routingKey,
                propriedades,
                mensagemBytes);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception captured on {Method} - {@DataPublished}",
                nameof(PublishMessage), new { serializedMessage, routingKey });

            return false;
        }
    }
}

