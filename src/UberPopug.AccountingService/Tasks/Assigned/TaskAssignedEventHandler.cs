using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Assigned
{
    public class TaskAssignedEventHandler : ITaskAssignedEventHandler
    {
        public async Task HandleAsync(TaskAssignedEvent @event)
        {
            
        }
    }
}