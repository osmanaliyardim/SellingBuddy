using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Events;
using System.Net.Sockets;
using System.Text;

namespace SellingBuddy.EventBus.RabbitMQ;

public class EventBusRabbitMQ : BaseEventBus
{
    RabbitMQPersistentConnection persistentConnection;
    private readonly IConnectionFactory connectionFactory;
    private readonly IModel consumerChannel;

    public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
    {
        if (eventBusConfig.Connection != null)
        {
            var connectionJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
            {
                // Self referencing loop detected for property
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
        }
        else connectionFactory = new ConnectionFactory();

        persistentConnection = new RabbitMQPersistentConnection(connectionFactory, EventBusConfig.ConnectionRetryCount);

        consumerChannel = CreateConsumerChannel();

        SubscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
    }

    public override void Publish(IntegrationEvent @event)
    {
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(EventBusConfig.ConnectionRetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (exception, time) =>
            {
                // ToDo: log
            });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);

        // Ensure exchange exists while publishing
        consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2; // Persistence

            // Ensure if queue exists while publishing
            //consumerChannel.QueueDeclare(
            //        queue: GetSubName(eventName),
            //        durable: true,
            //        exclusive: false,
            //        autoDelete: false,
            //        arguments: null);

            //consumerChannel.QueueBind(
            //        queue: GetSubName(eventName),
            //        exchange: EventBusConfig.DefaultTopicName,
            //        routingKey: eventName);

            consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
        });
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            // Ensure if queue exists while consuming
            consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            consumerChannel.QueueBind(queue: GetSubName(eventName),
                                    exchange: EventBusConfig.DefaultTopicName,
                                    routingKey: eventName);
        }

        SubscriptionManager.AddSubscription<T, TH>();

        StartBasicConsume(eventName);
    }

    public override void UnSubscribe<T, TH>()
    {
        SubscriptionManager.RemoveSubscription<T, TH>();
    }

    private void SubscriptionManager_OnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);

        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        consumerChannel.QueueUnbind(
                queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName
            );

        if (SubscriptionManager.isEmpty)
        {
            consumerChannel.Close();
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!persistentConnection.IsConnected)
        {
            persistentConnection.TryConnect();
        }

        var channel = persistentConnection.CreateModel();

        channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                                type: "direct");

        return channel;
    }

    private void StartBasicConsume(string eventName)
    {
        if (consumerChannel != null)
        {
            var consumer = new /*Async*/EventingBasicConsumer(consumerChannel);

            consumer.Received += Consumer_Received;

            consumerChannel.BasicConsume(
                    queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer
                );
        }
    }

    private async void Consumer_Received(object? sender, BasicDeliverEventArgs eventArgs)
    {
        var eventName = eventArgs.RoutingKey;
        eventName = ProcessEventName(eventName);
        var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception ex)
        {
            // ToDo: log
        }

        consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
    }
}
