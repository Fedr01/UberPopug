using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Created
{
    public class TaskCreatedEventV2Handler : ITaskCreatedEventV2Handler
    {
        private readonly AccountingDbContext _dbContext;

        public TaskCreatedEventV2Handler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(TaskCreatedEvent.V2 @event)
        {
        }
    }
}