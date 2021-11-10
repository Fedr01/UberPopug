using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UberPopug.Common.Constants;
using UberPopug.Common.Interfaces;
using UberPopug.TaskTrackerService.Tasks.Messages;
using UberPopug.TaskTrackerService.Users;

namespace UberPopug.TaskTrackerService.Tasks
{
    public class TasksManager : ITasksManager
    {
        private readonly TaskTrackerDbContext _context;
        private readonly IKafkaProducer _producer;
        private readonly Random _random = new();

        private List<User> _usersCache = new();

        public System.Threading.Tasks.Task<List<Task>> GetAllAsync()
        {
            return _context.Tasks.ToListAsync();
        }

        public TasksManager(TaskTrackerDbContext context, IKafkaProducer producer)
        {
            _context = context;
            _producer = producer;
        }

        public async System.Threading.Tasks.Task CreateAsync(CreateTaskCommand command)
        {
            var task = new Task(command.Description);
            _context.Tasks.Add(task);

            var users = await _context.Users.ToListAsync();

            var random = new Random();
            var assignedTo = users[random.Next(0, users.Count)];
            task.AssignTo(assignedTo);

            await _context.SaveChangesAsync();

            await _producer.ProduceAsync(KafkaTopics.TasksStream, new TaskCreatedEvent
            {
                PublicId = task.PublicId,
                Description = task.Description
            });

            await _producer.ProduceAsync(KafkaTopics.Tasks, new TaskCreatedEvent
            {
                PublicId = task.PublicId,
                Description = task.Description
            });
        }

        public async System.Threading.Tasks.Task CompleteAsync(int taskId)
        {
            var task = await _context.Tasks.FirstAsync(t => t.Id == taskId);
            task.Complete();

            await _context.SaveChangesAsync();

            await _producer.ProduceAsync(KafkaTopics.TasksLifecycle, new TaskCompletedEvent
            {
                PublicId = task.PublicId
            });
        }

        public async System.Threading.Tasks.Task<List<Task>> AssignAllAsync()
        {
            var tasks = await _context.Tasks.ToListAsync();

            foreach (var task in tasks)
            {
                await AssignAsync(task);
            }

            await _context.SaveChangesAsync();

            return tasks;
        }

        private async System.Threading.Tasks.Task AssignAsync(Task task)
        {
            if (!_usersCache.Any())
            {
                _usersCache = await _context.Users.ToListAsync();
            }

            var assignedTo = _usersCache[_random.Next(0, _usersCache.Count)];
            task.AssignTo(assignedTo);

            await _context.SaveChangesAsync();

            await _producer.ProduceAsync(KafkaTopics.TasksLifecycle, new TaskAssignedEvent
            {
                PublicId = task.PublicId,
                AssignedToEmail = task.AssignedToEmail
            });
        }
    }

    public interface ITasksManager
    {
        System.Threading.Tasks.Task<List<Task>> GetAllAsync();
        System.Threading.Tasks.Task CreateAsync(CreateTaskCommand command);
        System.Threading.Tasks.Task CompleteAsync(int taskId);
        System.Threading.Tasks.Task<List<Task>> AssignAllAsync();
    }

    public class CreateTaskCommand
    {
        public string Description { get; set; }
    }
}