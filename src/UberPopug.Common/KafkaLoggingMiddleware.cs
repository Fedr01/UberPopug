using System;
using System.Threading.Tasks;
using KafkaFlow;
using Microsoft.Extensions.Logging;
using UberPopug.SchemaRegistry.Schemas;

namespace UberPopug.Common
{
    public class KafkaLoggingMiddleware : IMessageMiddleware
    {
        private readonly ILogger<KafkaLoggingMiddleware> _logger;

        public KafkaLoggingMiddleware(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<KafkaLoggingMiddleware>();
        }

        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            var messageValue = context.Message.Value as IEvent;

            var topic = context.ProducerContext == null
                ? context.ConsumerContext.Topic
                : context.ProducerContext.Topic;

            if (context.ConsumerContext != null)
            {
                _logger.LogInformation(
                    $"Consuming {messageValue?.MetaData.EventName} {messageValue?.MetaData.Version}" +
                    $" with id {messageValue?.MetaData.EventId}" +
                    $" from {topic}" +
                    $" produced by {messageValue?.MetaData.Producer}" +
                    $" at {messageValue?.MetaData.EventTime:G}");
            }
            else if (context.ProducerContext != null)
            {
                _logger.LogInformation(
                    $"Producing {messageValue?.MetaData.EventName} {messageValue?.MetaData.Version}" +
                    $" with id {messageValue?.MetaData.EventId}" +
                    $" to {topic}" +
                    $" by {messageValue?.MetaData.Producer}" +
                    $" at {messageValue?.MetaData.EventTime:G}");
            }

            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Failed to process {messageValue?.MetaData.EventName} {messageValue?.MetaData.Version}" +
                    $" with id {messageValue?.MetaData.EventId}" +
                    $" to {topic}" +
                    $" by {messageValue?.MetaData.Producer}" +
                    $" at {messageValue?.MetaData.EventTime:G}");
            }
        }
    }
}