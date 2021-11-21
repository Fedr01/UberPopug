using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using KafkaFlow.Configuration;
using KafkaFlow.Serializer.SchemaRegistry;

namespace UberPopug.Common
{
    public static class KafkaConfigurationBuilderExtensions
    {
        public static IProducerMiddlewareConfigurationBuilder AddTypedSchemaRegistryJsonSerializer(
            this IProducerMiddlewareConfigurationBuilder middlewares,
            JsonSerializerConfig config = null)
        {
            return middlewares.AddSerializer(
                resolver => new ConfluentJsonSerializer(resolver, config),
                _ => new KafkaMessageTypeResolver());
        }

        public static IConsumerMiddlewareConfigurationBuilder AddTypedSchemaRegistryJsonSerializer(
            this IConsumerMiddlewareConfigurationBuilder middlewares)
        {
            return middlewares.AddSerializer(
                resolver => new ConfluentJsonSerializer(resolver),
                _ => new KafkaMessageTypeResolver());
        }
    }
}