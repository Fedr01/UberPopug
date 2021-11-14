using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Completed
{
    public class TaskCompletedEventHandler : ITaskCompletedEventHandler
    {
        private readonly AccountingDbContext _dbContext;

        public TaskCompletedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(TaskCompletedEvent ev)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.User)
                .FirstAsync(t => t.PublicId == ev.PublicId);
            
            task.Complete();
            await _dbContext.SaveChangesAsync();
        }
    }
}