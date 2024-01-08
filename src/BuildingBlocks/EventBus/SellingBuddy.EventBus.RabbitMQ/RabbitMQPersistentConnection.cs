using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace SellingBuddy.EventBus.RabbitMQ;

public class RabbitMQPersistentConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly int _retryCount;
    private IConnection rabbitMQConnection;
    private object lock_object = new object();
    private bool isDisposed;

    public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        _connectionFactory = connectionFactory;
        _retryCount = retryCount;
    }

    public bool IsConnected => rabbitMQConnection != null && rabbitMQConnection.IsOpen;

    public IModel CreateModel()
    {
        return rabbitMQConnection.CreateModel();
    }
    public void Dispose()
    {
        isDisposed = true;
        rabbitMQConnection.Dispose();
    }

    public bool TryConnect()
    {
        lock(lock_object)
        {
            var policy = Policy
                .Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, time) =>
                {

                });

            policy.Execute(() =>
            {
                rabbitMQConnection = _connectionFactory.CreateConnection();
            });

            if (IsConnected)
            {
                rabbitMQConnection.ConnectionShutdown += RabbitMQConnection_ConnectionShutdown;
                rabbitMQConnection.CallbackException += RabbitMQConnection_CallbackException;
                rabbitMQConnection.ConnectionBlocked += RabbitMQConnection_ConnectionBlocked;

                // ToDo: log

                return true;
            }

            return false;
        }
    }

    private void RabbitMQConnection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        // ToDo: log

        if (!isDisposed) TryConnect();
    }

    private void RabbitMQConnection_CallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        // ToDo: log

        if (!isDisposed) TryConnect();
    }

    private void RabbitMQConnection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        // ToDo: log

        if (!isDisposed) TryConnect();
    }
}
