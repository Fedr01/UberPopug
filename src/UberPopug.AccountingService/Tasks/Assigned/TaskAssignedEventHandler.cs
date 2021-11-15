using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Assigned
{
    public class TaskAssignedEventHandler : IMessageHandler<TaskAssignedEvent>
    {
        private readonly AccountingDbContext _dbContext;

        public TaskAssignedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(IMessageContext context, TaskAssignedEvent message)
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == message.PublicId);
            var user = await _dbContext.Users.FirstAsync(u => u.Email == message.AssignedToEmail);

            task.AssignTo(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}