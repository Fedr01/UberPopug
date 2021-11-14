using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task HandleAsync(TaskCreatedEvent.V2 ev)
        {
            await TasksCreatedSemaphore.Semaphore.WaitAsync();

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == ev.PublicId);
            if (task == null)
            {
                task = new TrackerTask("", null, ev.PublicId);
                _dbContext.Tasks.Add(task);
            }

            task.Estimate();
            await _dbContext.SaveChangesAsync();

            TasksCreatedSemaphore.Semaphore.Release();
        }
    }
}