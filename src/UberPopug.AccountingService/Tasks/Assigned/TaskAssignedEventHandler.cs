using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Assigned
{
    public class TaskAssignedEventHandler : ITaskAssignedEventHandler
    {
        private readonly AccountingDbContext _dbContext;

        public TaskAssignedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(TaskAssignedEvent ev)
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == ev.PublicId);
            var user = await _dbContext.Users.FirstAsync(u => u.Email == ev.AssignedToEmail);
            
            task.AssignTo(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}