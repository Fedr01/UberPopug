using System.Threading.Tasks;

namespace UberPopug.Common.Interfaces
{
    public interface IKafkaProducer
    {
        Task ProduceAsync(string topic, object value);
    }
}