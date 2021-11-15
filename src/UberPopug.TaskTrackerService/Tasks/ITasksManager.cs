using System.Collections.Generic;
using System.Threading.Tasks;

namespace UberPopug.TaskTrackerService.Tasks
{
    public interface ITasksManager
    {
        Task<List<TrackerTask>> GetAllAsync();
        Task CreateAsync(CreateTaskCommand command);
        Task CompleteAsync(int taskId);
        Task<List<TrackerTask>> AssignAllAsync();
    }
}