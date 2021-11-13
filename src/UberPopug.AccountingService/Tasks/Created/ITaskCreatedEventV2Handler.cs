using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Created
{
    public interface ITaskCreatedEventV2Handler : IEventHandler<TaskCreatedEvent.V2>
    {
    }
}