using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using UberPopug.Common.Interfaces;

namespace UberPopug.Common.Kafka
{
    public class KafkaProducer : IDisposable, IKafkaProducer
    {
        private readonly IProducer<long, string> _producer;

        public KafkaProducer(ProducerConfig config)
        {
            _producer = new ProducerBuilder<long, string>(config)
                .SetKeySerializer(Serializers.Int64)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
        }


        public async Task ProduceAsync(string topic, object value)
        {
            await _producer.ProduceAsync(topic, new Message<long, string>
            {
                Key = DateTime.UtcNow.Ticks,
                Value = JsonSerializer.Serialize(value)
            });
        }

        public void Dispose()
        {
            _producer.Flush();
            _producer.Dispose();
        }
    }
}