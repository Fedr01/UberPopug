using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Assigned
{
    public interface ITaskAssignedEventHandler : IEventHandler<TaskAssignedEvent>
    {
    }
}