using System;
using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks;

namespace UberPopug.AccountingService.Tasks.Completed
{
    public class TaskCompletedEventHandler : IMessageHandler<TaskCompletedEvent>
    {
        private readonly AccountingDbContext _dbContext;

        public TaskCompletedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(IMessageContext context, TaskCompletedEvent message)
        {
            var task = await _dbContext.Tasks
                .Include(t => t.Account)
                .FirstAsync(t => t.PublicId == message.PublicId);
            
            task.Complete();
            await _dbContext.SaveChangesAsync();
            context.ConsumerContext.StoreOffset();
        }
    }
}