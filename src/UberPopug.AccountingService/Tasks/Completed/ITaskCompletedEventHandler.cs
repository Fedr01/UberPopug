using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Completed
{
    public interface ITaskCompletedEventHandler : IEventHandler<TaskCompletedEvent>
    {
    }
}