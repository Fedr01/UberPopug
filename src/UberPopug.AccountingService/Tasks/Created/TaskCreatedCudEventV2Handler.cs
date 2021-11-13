using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;

namespace UberPopug.AccountingService.Tasks.Created
{
    public class TaskCreatedCudEventV2Handler : ITaskCreatedCudEventV2Handler
    {
        private readonly AccountingDbContext _dbContext;

        public TaskCreatedCudEventV2Handler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(TaskCreatedCudEvent.V2 @event)
        {
            _dbContext.Tasks.Add(new TrackerTask(@event.Title, @event.JiraId, @event.PublicId));
            await _dbContext.SaveChangesAsync();
        }
    }
}