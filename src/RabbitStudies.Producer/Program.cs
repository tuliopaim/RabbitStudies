using Microsoft.Extensions.Logging;
using RabbitStudies.Contracts;
using RabbitStudies.Producer;
using RabbitStudies.RabbitMq;

var connectionFactory = RabbitMqConnectionFactory.ConnectionFactory;

var rabbitMqConnection = new RabbitMqConnection(connectionFactory);
var rabbitProducer = new RabbitMqProducer(
    rabbitMqConnection, 
    new Logger<RabbitMqProducer>(new LoggerFactory())); 

Console.WriteLine("Rabbit producer started");
var messageQuantity = GetMessageQuantity();

for (var messageIndex = 0; messageIndex < messageQuantity; messageIndex++)
{
    rabbitProducer.Publish(new HelloWorldMessage(), MessageType.HELLO_WORLD, "QUEUE_A");
}

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


