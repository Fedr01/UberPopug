using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using UberPopug.Common.Interfaces;

namespace UberPopug.Common.Producer
{
    /// <summary>
    /// Base class for implementing Kafka Producer.
    /// </summary>
    /// <typeparam name="TKey">Indicates message's key in Kafka topic</typeparam>
    /// <typeparam name="TValue">Indicates message's value in Kafka topic</typeparam>
    public class KafkaProducer<TKey, TValue> : IDisposable, IKafkaProducer<TKey,TValue> where TValue : class
    {
        private readonly IProducer<TKey, TValue> _producer;

        /// <summary>
        /// Initializes the producer
        /// </summary>
        /// <param name="config"></param>
        public KafkaProducer(ProducerConfig config)
        {
            _producer = new ProducerBuilder<TKey, TValue>(config).SetValueSerializer(new KafkaSerializer<TValue>()).Build();
        }

        /// <summary>
        /// Triggered when the service creates Kafka topic.
        /// </summary>
        /// <param name="topic">Indicates topic name</param>
        /// <param name="key">Indicates message's key in Kafka topic</param>
        /// <param name="value">Indicates message's value in Kafka topic</param>
        /// <returns></returns>
        public async Task ProduceAsync(string topic,TKey key, TValue value)
        {
            await _producer.ProduceAsync(topic, new Message<TKey, TValue> { Key = key, Value = value });
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}
