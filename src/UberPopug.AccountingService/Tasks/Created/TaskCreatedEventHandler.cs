using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Created
{
    public class TaskCreatedEventHandler : IMessageHandler<TaskCreatedEvent>
    {
        private readonly AccountingDbContext _dbContext;

        public TaskCreatedEventHandler(AccountingDbContext dbContext)
        {
            
            _dbContext = dbContext;
        }

        public async Task Handle(IMessageContext context, TaskCreatedEvent message)
        {
            await TasksCreatedSemaphore.Semaphore.WaitAsync();

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == message.PublicId);
            if (task == null)
            {
                task = new TrackerTask("", null, message.PublicId);
                _dbContext.Tasks.Add(task);
            }

            task.Estimate();
            await _dbContext.SaveChangesAsync();
            
            context.ConsumerContext.StoreOffset();

            TasksCreatedSemaphore.Semaphore.Release();
        }
    }
}