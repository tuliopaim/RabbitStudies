using RabbitMQ.Client;

namespace RabbitStudies.RabbitMq;

public class RabbitMqConnection
{
    private readonly IConnectionFactory _factory;
    private readonly object _connectionLocker = new();

    private IConnection _connection;

    public RabbitMqConnection(IConnectionFactory factory)
    {
        _factory = factory;
        _connection = ObterConexao();
    }

    public IConnection Connection => ObterConexao();

    private IConnection ObterConexao()
    {
        if (_connection is not { IsOpen: true })
        {
            lock (_connectionLocker)
            {
                if (_connection is not { IsOpen: true })
                {
                    _connection?.Dispose();
                    _connection = _factory.CreateConnection();
                }
            }
        }

        return _connection;
    }
}
