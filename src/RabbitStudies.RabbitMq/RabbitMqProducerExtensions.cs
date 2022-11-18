using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using Polly;
using Polly.Retry;

namespace RabbitStudies.RabbitMq;
public static class RabbitMqProducerExtensions
{
    public static void Publish<TMessage>(
        this RabbitMqConnection connection,
        TMessage message,
        MessageType messageType,
        string routingKey,
        RetryPolicy? retryPolicy = null)
    {
        var serializedMessage = JsonConvert.SerializeObject(message);

        var messageBase = new RabbitMessage(serializedMessage, messageType);

        retryPolicy ??= RetryPolicy;
        
        retryPolicy.Execute(() =>
        {
            connection.PublishMessage(JsonConvert.SerializeObject(messageBase), routingKey);
        });
    }

    private static void PublishMessage(
        this RabbitMqConnection connectionPool,
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
    };

    private static  readonly RetryPolicy RetryPolicy = Policy
        .Handle< Exception>()
        .WaitAndRetry(sleepDurations: SleepsBetweenRetries);
}

