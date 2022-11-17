using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace RabbitStudies.RabbitMq;
public class RabbitMqProducer
{
    private readonly RabbitMqConnection _rabbitConnection;
    private readonly ILogger<RabbitMqProducer> _logger;

    public RabbitMqProducer(RabbitMqConnection rabbitConnection, ILogger<RabbitMqProducer> logger)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
    }

    private IConnection Connection => _rabbitConnection.Connection;

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
            var channel = Connection.CreateModel();
            
            var mensagemBytes = Encoding.UTF8.GetBytes(serializedMessage);

            var propriedades = channel.CreateBasicProperties();

            propriedades.Persistent = true;
            propriedades.CreateRetryCountHeader();
            
            channel.BasicPublish("",
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

