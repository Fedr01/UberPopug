using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;

namespace UberPopug.AccountingService.Tasks.Created
{
    public class TaskCreatedCudEventV2Handler : IMessageHandler<TaskCreatedCudEvent.V2>
    {
        private readonly AccountingDbContext _dbContext;


        public TaskCreatedCudEventV2Handler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(IMessageContext context, TaskCreatedCudEvent.V2 message)
        {
            await TasksCreatedSemaphore.Semaphore.WaitAsync();

            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == message.PublicId);
            if (task == null)
            {
                _dbContext.Tasks.Add(new TrackerTask(message.Title, message.JiraId, message.PublicId));
            }
            else
            {
                task.UpdateTitle(message.Title, message.JiraId);
            }

            await _dbContext.SaveChangesAsync();

            TasksCreatedSemaphore.Semaphore.Release();
        }
    }
}