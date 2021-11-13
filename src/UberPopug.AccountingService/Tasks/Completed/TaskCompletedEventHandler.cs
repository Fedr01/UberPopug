using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Completed
{
    public class TaskCompletedEventHandler : ITaskCompletedEventHandler
    {
        public async Task HandleAsync(TaskCompletedEvent @event)
        {
        }
    }
}