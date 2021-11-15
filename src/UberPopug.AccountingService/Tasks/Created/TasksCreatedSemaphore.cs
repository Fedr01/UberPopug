using System.Threading;

namespace UberPopug.AccountingService.Tasks.Created
{
    public static class TasksCreatedSemaphore
    {
        public static SemaphoreSlim Semaphore = new(1);
    }
}