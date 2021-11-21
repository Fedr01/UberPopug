using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using UberPopug.SchemaRegistry.Schemas.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly TaskTrackerDbContext _context;
        private readonly IMessageProducer<TaskCreatedCudEvent> _createdCudEventProducer;
        private readonly IMessageProducer<TaskCreatedEvent> _createdProducer;
        private readonly IMessageProducer<TaskCompletedEvent> _completedProducer;
        private readonly IMessageProducer<TaskAssignedEvent> _assignedProducer;
        private readonly Random _random = new();

        private List<User> _usersCache = new();

        public TasksManager(TaskTrackerDbContext context,
            IMessageProducer<TaskCreatedEvent> createdProducer,
            IMessageProducer<TaskCompletedEvent> completedProducer,
            IMessageProducer<TaskAssignedEvent> assignedProducer,
            IMessageProducer<TaskCreatedCudEvent> createdCudEventProducer)
        {
            _context = context;
            _createdProducer = createdProducer;
            _completedProducer = completedProducer;
            _assignedProducer = assignedProducer;
            _createdCudEventProducer = createdCudEventProducer;
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

            await _createdCudEventProducer.ProduceAsync(Guid.NewGuid().ToString(),
                new TaskCreatedCudEvent(task.PublicId, task.Title, task.JiraId));

            await _createdProducer.ProduceAsync(Guid.NewGuid().ToString(), new TaskCreatedEvent(task.PublicId));

            await AssignAsync(task);
        }

        public async Task CompleteAsync(int taskId)
        {
            var task = await _context.Tasks.FirstAsync(t => t.Id == taskId);
            task.Complete();

            await _context.SaveChangesAsync();

            await _completedProducer.ProduceAsync(Guid.NewGuid().ToString(), new TaskCompletedEvent(task.PublicId));
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

            await _assignedProducer.ProduceAsync(Guid.NewGuid().ToString(),
                new TaskAssignedEvent(trackerTask.PublicId, trackerTask.AssignedToEmail));
        }
    }
}