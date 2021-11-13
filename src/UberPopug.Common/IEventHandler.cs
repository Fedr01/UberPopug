using System.Threading.Tasks;
using UberPopug.SchemaRegistry;

namespace UberPopug.Common
{
    public interface IEventHandler<TEvent>
    {
        Task HandleAsync(TEvent @event);
    }
}