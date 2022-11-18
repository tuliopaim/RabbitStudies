using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace RabbitStudies.RabbitMq;

public class RabbitMqConnectionPool
{
    private static readonly object LockInstance = new();

    private readonly IConnectionFactory _connectionFactory;
    private readonly List<IConnection> _connections = new();

    private const uint PoolSize = 5;

    public RabbitMqConnectionPool(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IConnection Connection => _retryPolicy.Execute(GetConnection);
    private IConnection GetConnection()
    {
        lock (LockInstance)
        {
            EnsurePoolSize();

           return _connections.First(c => c.IsOpen);
        }
    }

    private void EnsurePoolSize()
    {
        _connections.RemoveAll(c => !c.IsOpen);

        var connectionsToCreate = PoolSize - (uint)_connections.Count;

        for (var i = 0; i < connectionsToCreate; i++)
        {
            _connections.Add(_connectionFactory.CreateConnection());
        }
    }
    
    private static readonly IEnumerable<TimeSpan> SleepsBetweenRetries = new List<TimeSpan>
    {
        TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 
        TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(8),
    };

    private readonly RetryPolicy _retryPolicy = Policy
        .Handle< Exception>()
        .WaitAndRetry(sleepDurations: SleepsBetweenRetries);
}