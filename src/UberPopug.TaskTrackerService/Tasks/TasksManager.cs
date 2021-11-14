using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UberPopug.Common.Constants;
using UberPopug.Common.Interfaces;
using UberPopug.SchemaRegistry.Schemas.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly TaskTrackerDbContext _context;
        private readonly IKafkaProducer _producer;
        private readonly Random _random = new();

        private List<User> _usersCache = new();

        public TasksManager(TaskTrackerDbContext context, IKafkaProducer producer)
        {
            _context = context;
            _producer = producer;
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

            await _producer.ProduceAsync(KafkaTopics.TasksStream, new TaskCreatedCudEvent.V2
            {
                PublicId = task.PublicId,
                Title = task.Title,
                JiraId = task.JiraId
            });

            await _producer.ProduceAsync(KafkaTopics.Tasks, new TaskCreatedEvent.V2
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

            await _producer.ProduceAsync(KafkaTopics.Tasks, new TaskCompletedEvent
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

            await _producer.ProduceAsync(KafkaTopics.Tasks, new TaskAssignedEvent
            {
                PublicId = trackerTask.PublicId,
                AssignedToEmail = trackerTask.AssignedToEmail
            });
        }
    }
}