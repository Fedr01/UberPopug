using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task HandleAsync(TaskCreatedCudEvent.V2 ev)
        {
            await TasksCreatedSemaphore.Semaphore.WaitAsync();
            
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == ev.PublicId);
            if (task == null)
            {
                _dbContext.Tasks.Add(new TrackerTask(ev.Title, ev.JiraId, ev.PublicId));
            }
            else
            {
                task.UpdateTitle(ev.Title, ev.JiraId);
            }
            await _dbContext.SaveChangesAsync();
            
            TasksCreatedSemaphore.Semaphore.Release();
        }
    }
}