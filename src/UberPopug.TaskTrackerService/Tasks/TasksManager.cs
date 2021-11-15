using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow.Producers;
using Microsoft.EntityFrameworkCore;
using UberPopug.Common.Constants;
using UberPopug.SchemaRegistry.Schemas.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly TaskTrackerDbContext _context;
        private readonly IProducerAccessor _producers;
        private readonly Random _random = new();

        private List<User> _usersCache = new();

        public TasksManager(TaskTrackerDbContext context, IProducerAccessor producers)
        {
            _context = context;
            _producers = producers;
        }


        public Task<List<TrackerTask>> GetAllAsync()
        {
            return _context.Tasks.ToListAsync();
        }

        public async Task CreateAsync(CreateTaskCommand command)
        {
            var task = new TrackerTask(command.Title, command.JiraId);
            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            await _producers[KafkaTopics.TasksStream].ProduceAsync(Guid.NewGuid().ToString(), new TaskCreatedCudEvent.V2
            {
                PublicId = task.PublicId,
                Title = task.Title,
                JiraId = task.JiraId
            });

            await _producers[KafkaTopics.Tasks].ProduceAsync(Guid.NewGuid().ToString(), new TaskCreatedEvent.V2
            {
                PublicId = task.PublicId
            });

            await AssignAsync(task);
        }

        public async Task CompleteAsync(int taskId)
        {
            var task = await _context.Tasks.FirstAsync(t => t.Id == taskId);
            task.Complete();

            await _context.SaveChangesAsync();

            await _producers[KafkaTopics.Tasks].ProduceAsync(Guid.NewGuid().ToString(), new TaskCompletedEvent
            {
                PublicId = task.PublicId
            });
        }

        public async Task<List<TrackerTask>> AssignAllAsync()
        {
            var tasks = await _context.Tasks.ToListAsync();

            foreach (var task in tasks)
            {
                await AssignAsync(task);
            }

            await _context.SaveChangesAsync();

            return tasks;
        }

        private async Task AssignAsync(TrackerTask trackerTask)
        {
            if (!_usersCache.Any())
            {
                _usersCache = await _context.Users.ToListAsync();
            }

            var assignedTo = _usersCache[_random.Next(0, _usersCache.Count)];
            trackerTask.AssignTo(assignedTo);

            await _context.SaveChangesAsync();

            await _producers[KafkaTopics.Tasks].ProduceAsync(Guid.NewGuid().ToString(), new TaskAssignedEvent
            {
                PublicId = trackerTask.PublicId,
                AssignedToEmail = trackerTask.AssignedToEmail
            });
        }
    }
}