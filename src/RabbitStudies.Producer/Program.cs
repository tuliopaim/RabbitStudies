using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitStudies.Contracts;
using RabbitStudies.RabbitMq;
using RabbitStudies.RabbitMq.Settings;

var rabbitSettings = new RabbitMqSettings
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

var loggerFactory = new LoggerFactory();
var logger = new Logger<RabbitMqProducer>(loggerFactory);

var connectionFactory = new ConnectionFactory
{
    HostName = rabbitSettings.HostName,
    Port = rabbitSettings.Port,
    UserName = rabbitSettings.UserName,
    Password = rabbitSettings.Password,
    DispatchConsumersAsync = true,
    ConsumerDispatchConcurrency = 1,
    UseBackgroundThreadsForIO = false
};

var rabbitMqConnection = new RabbitMqConnection(connectionFactory);
var rabbitProducer = new RabbitMqProducer(rabbitMqConnection, logger);

Console.WriteLine("Rabbit producer, press any key to produce a HelloWorldMessage");

var messageIndex = 0;

while (true)
{
    //Console.ReadKey();

    var message = new HelloWorldMessage();

    var published = rabbitProducer.Publish(message, MessageType.HELLO_WORLD, "QUEUE_A");

    if (published)
    {
        Console.WriteLine($"Message {++messageIndex} produced Id: {message.Id}");
    }
}
