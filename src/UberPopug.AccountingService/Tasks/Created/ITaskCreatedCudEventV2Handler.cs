using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;

namespace UberPopug.AccountingService.Tasks.Created
{
    public interface ITaskCreatedCudEventV2Handler : IEventHandler<TaskCreatedCudEvent.V2>
    {
    }
}