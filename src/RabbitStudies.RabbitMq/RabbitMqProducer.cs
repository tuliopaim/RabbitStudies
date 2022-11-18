using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Polly;
using Polly.Retry;

namespace RabbitStudies.RabbitMq;
public static class RabbitMqProducer
{
    public static void Publish<TMessage>(
        this RabbitMqConnectionPool connection,
        TMessage message,
        MessageType messageType,
        string routingKey)
    {
        var serializedMessage = JsonConvert.SerializeObject(message);

        var messageBase = new RabbitMessage(serializedMessage, messageType);

        RetryPolicy.Execute(() =>
        {
            connection.PublishMessage(JsonConvert.SerializeObject(messageBase), routingKey);
        });
    }

    private static void PublishMessage(
        this RabbitMqConnectionPool connectionPool,
        string serializedMessage,
        string routingKey)
    {
        var connection = connectionPool.Connection;
        var mensagemBytes = Encoding.UTF8.GetBytes(serializedMessage);

        var channel = connection.CreateModel(); 
        var propriedades = channel.CreateBasicProperties();

        propriedades.Persistent = true;
        propriedades.CreateRetryCountHeader();
        
        channel.BasicPublish("",
            routingKey,
            propriedades,
            mensagemBytes);
    }
    
    private static readonly IEnumerable<TimeSpan> SleepsBetweenRetries = new List<TimeSpan>
    {
        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 
        TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(8),
    };

    private static  readonly RetryPolicy RetryPolicy = Policy
        .Handle< Exception>()
        .WaitAndRetry(sleepDurations: SleepsBetweenRetries);
}

