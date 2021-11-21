using System;
using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using UberPopug.SchemaRegistry.Schemas;

namespace UberPopug.Common
{
    public class KafkaDeadLetterQueueMiddleware : IMessageMiddleware
    {
        public static string Producer = "DeadLetterProducer";
        public static string Topic = "dead-letters";

        private readonly ILogger<KafkaDeadLetterQueueMiddleware> _logger;
        private readonly IMessageProducer _producer;

        public KafkaDeadLetterQueueMiddleware(ILoggerFactory factory, IProducerAccessor accessor)
        {
            _producer = accessor[Producer];
            _logger = factory.CreateLogger<KafkaDeadLetterQueueMiddleware>();
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            try
            {
                await next(context);
            }

            catch (Exception e)
            {
                var messageValue = context.Message.Value as IEvent;

                var topic = context.ProducerContext == null
                    ? context.ConsumerContext.Topic
                    : context.ProducerContext.Topic;

                _logger.LogCritical(e,
                    $" Dead letter! Failed to process {messageValue?.MetaData.EventName} {messageValue?.MetaData.Version}" +
                    $" with id {messageValue?.MetaData.EventId}" +
                    $" to {topic}" +
                    $" by {messageValue?.MetaData.Producer}" +
                    $" at {messageValue?.MetaData.EventTime:G}");

                await _producer.ProduceAsync(Topic, messageValue.MetaData.EventId.ToString(), messageValue);
            }
        }
    }
}