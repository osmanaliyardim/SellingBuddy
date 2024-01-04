using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SellingBuddy.EventBus.Base;
using SellingBuddy.EventBus.Base.Abstraction;
using SellingBuddy.EventBus.Base.Events;
using System.Text;

namespace SellingBuddy.EventBus.AzureServiceBus;

public class EventBusServiceBus : BaseEventBus
{
    private ITopicClient topicClient;
    private ManagementClient managementClient;
    private ILogger logger;

    public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
    {
        logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
        managementClient = new ManagementClient(eventBusConfig.EventBusConnectionString);
        topicClient = CreateTopicClient();
    }

    private ITopicClient CreateTopicClient()
    {
        if (topicClient == null || topicClient.IsClosedOrClosing)
        {
            topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default);
        }

        // Ensure that topic already exists or not
        if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
        {
            managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
        }

        return topicClient;
    }

    public override void Publish(IntegrationEvent @event)
    {
        // Ex: OrderCreatedIntegratedEvent
        var eventName = @event.GetType().Name; 

        // Ex: OrderCreated
        eventName = ProcessEventName(eventName);

        var eventStr = JsonConvert.SerializeObject(@event);
        var bodyArr = Encoding.UTF8.GetBytes(eventStr);

        var message = new Message()
        {
            MessageId = Guid.NewGuid().ToString(),
            Body = bodyArr,
            Label = eventName
        };

        topicClient.SendAsync(message).GetAwaiter().GetResult();
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubscriptionManager.HasSubscriptionsForEvent(eventName))
        {
            var subscriptionClient = CreateSubscriptionClientIfNotExist(eventName);

            RegisterSubscriptionClientMessageHandler(subscriptionClient);
        }

        logger.LogInformation($"Subscribing to event {eventName} with {typeof(TH).Name}");

        SubscriptionManager.AddSubscription<T, TH>();
    }

    public override void UnSubscribe<T, TH>()
    {
        var eventName = typeof(T).Name;

        try
        {
            // Subscription will be there but we do not subscribe
            var subscriptionClient = CreateSubscriptionClient(eventName);

            subscriptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            logger.LogWarning($"The messaging entity {eventName} could not be found!");
        }

        logger.LogInformation($"Unsubscribing from event {eventName}");

        SubscriptionManager.RemoveSubscription<T, TH>();
    }

    private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
    {
        subscriptionClient.RegisterMessageHandler
        (
            async (message, token) =>
            {
                var eventName = $"{message.Label}";
                var messageData = Encoding.UTF8.GetString(message.Body);

                // Complete the message so that it is not received again.
                if (await ProcessEvent(ProcessEventName(eventName), messageData))
                {
                    await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                }
            },
            new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false }
        );
    }

    private Task ExceptionReceivedHandler (ExceptionReceivedEventArgs exceptionReceivedEventArgs)
    {
        var exception = exceptionReceivedEventArgs.Exception;
        var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

        logger.LogError(exception, $"ERROR Handling Message: {exception.Message} - Context: {context}");

        return Task.CompletedTask;
    }

    private SubscriptionClient CreateSubscriptionClientIfNotExist(string eventName)
    {
        var subClient = CreateSubscriptionClient(eventName);

        var exists = managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

        if (!exists)
        {
            managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
            RemoveDefaultRule(subClient);
        }

        CreateRuleIfNotExists(ProcessEventName(eventName), subClient);

        return subClient;
    }

    private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
    {
        bool ruleExists;

        try
        {
            var rule = managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName).GetAwaiter().GetResult();
            ruleExists = rule != null;
        }
        catch (MessagingEntityNotFoundException)
        {
            // Azure Management Client does not have RuleExists method
            ruleExists = false;
        }

        if (!ruleExists)
        {
            subscriptionClient.AddRuleAsync(new RuleDescription
            {
                Filter = new CorrelationFilter { Label = eventName },
                Name = eventName
            }).GetAwaiter().GetResult();
        }
    }

    private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
    {
        try
        {
            subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName).GetAwaiter().GetResult();
        }
        catch (MessagingEntityNotFoundException)
        {
            logger.LogWarning($"The messaging entity {RuleDescription.DefaultRuleName} could not be found!");
        }
    }

    private SubscriptionClient CreateSubscriptionClient(string eventName)
    {
        return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
    }

    public override void Dispose()
    {
        base.Dispose();

        topicClient.CloseAsync().GetAwaiter().GetResult();
        managementClient.CloseAsync().GetAwaiter().GetResult();
        topicClient = null;
        managementClient = null;
    }
}
