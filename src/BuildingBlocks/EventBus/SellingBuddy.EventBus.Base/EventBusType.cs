namespace SellingBuddy.EventBus.Base;

public enum EventBusType
{
    RabbitMQ = 0,
    AzureServiceBus = 1,
    Kafka = 2,
    ActiveMQ = 3,
    AmazonSQS = 4,
    GooglePubSub = 5,
    RedisStreams = 6
}
