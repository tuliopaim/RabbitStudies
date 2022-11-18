using RabbitStudies.Contracts;
using RabbitStudies.Producer;
using RabbitStudies.RabbitMq;

var connectionFactory = RabbitMqConnectionFactory.ConnectionFactory;

var rabbitMqConnection = new RabbitMqConnection(connectionFactory);

Console.WriteLine("Rabbit producer started");
var messageQuantity = GetMessageQuantity();

PublishMessages(messageQuantity, rabbitMqConnection);
//await ParalelPublishMessages(messageQuantity, rabbitMqConnection);

Console.WriteLine($"All messages published");

int GetMessageQuantity()
{
    while (true)
    {
        Console.WriteLine("Generate how many messages?");
        var messagesQtyStr = Console.ReadLine();
        
        if (int.TryParse(messagesQtyStr, out var messagesQty))
        {
            return messagesQty;
        }
        
        Console.WriteLine("Wrong input...");
    }
}

void PublishMessages(int i, RabbitMqConnection connectionPool)
{
    for (var messageIndex = 0; messageIndex < i; messageIndex++)
    {
        connectionPool.Publish(
            new HelloWorldMessage(),
            MessageType.HELLO_WORLD,
            "QUEUE_A");
    }
}

Task ParalelPublishMessages(int i, RabbitMqConnection connectionPool)
{
    var tasks = new List<Task>();
    for (var messageIndex = 0; messageIndex < i; messageIndex++)
    {
        tasks.Add(Task.Run(() =>
        {
            connectionPool.Publish(
                new HelloWorldMessage(),
                MessageType.HELLO_WORLD,
                "QUEUE_A");
        }));
    }

    return Task.WhenAll(tasks);
}