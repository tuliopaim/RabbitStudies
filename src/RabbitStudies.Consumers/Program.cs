using RabbitMQ.Client;
using RabbitStudies.Consumers;
using RabbitStudies.RabbitMq;
using RabbitStudies.RabbitMq.Settings;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var rabbitSettings = new RabbitMqSettings();

        context.Configuration.GetSection(nameof(RabbitMqSettings)).Bind(rabbitSettings);

        services.AddSingleton<IConnectionFactory>(x=> new ConnectionFactory
        {
            HostName = rabbitSettings.HostName,
            Port = rabbitSettings.Port,
            UserName = rabbitSettings.UserName,
            Password = rabbitSettings.Password,
            DispatchConsumersAsync = true,
            ConsumerDispatchConcurrency = 1,
            UseBackgroundThreadsForIO = false
        });

        services.AddSingleton<RabbitMqConnection>();

        services.AddHostedService<ConsumerA>();
    })
    .Build();

host.Run();
