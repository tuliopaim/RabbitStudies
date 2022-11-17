using RabbitMQ.Client;
using RabbitStudies.RabbitMq.Settings;

namespace RabbitStudies.Producer;

public static  class RabbitMqConnectionFactory
{
    private static readonly RabbitMqSettings RabbitMqSettings = new RabbitMqSettings
    {
        HostName = "localhost",
        Port = 5672,
        UserName = "guest",
        Password = "guest",
        RetrySettings = new()
        {
            Count = 3,
            DurationInSeconds = 2,
        }
    }; 
        
    public static readonly IConnectionFactory ConnectionFactory = new ConnectionFactory
    {
        HostName = RabbitMqSettings.HostName,
        Port = RabbitMqSettings.Port,
        UserName = RabbitMqSettings.UserName,
        Password = RabbitMqSettings.Password,
        DispatchConsumersAsync = true,
        ConsumerDispatchConcurrency = 1,
        UseBackgroundThreadsForIO = false
    };
}