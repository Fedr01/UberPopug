using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UberPopug.AccountingService.Tasks.Assigned;
using UberPopug.AccountingService.Tasks.Completed;
using UberPopug.AccountingService.Tasks.Created;
using UberPopug.AccountingService.Users;
using UberPopug.Common.Constants;
using UberPopug.SchemaRegistry;
using UberPopug.SchemaRegistry.Schemas.Tasks;
using UberPopug.SchemaRegistry.Schemas.Tasks.Cud;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.AccountingService.Tasks
{
    public class TasksConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _kafkaConsumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _topic;

        public TasksConsumer(ConsumerConfig config, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _topic = KafkaTopics.TasksStream;
            _kafkaConsumer = new ConsumerBuilder<string, string>(config).Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
                try
                {
                    var result = _kafkaConsumer.Consume(cancellationToken);
                    var ev = JsonSerializer.Deserialize<Event>(result.Message.Value);
                    using var scope = _serviceScopeFactory.CreateScope();
                    switch (ev.MetaData.EventName)
                    {
                        case nameof(TaskCreatedEvent):
                            var createdEventHandler = scope.ServiceProvider.GetRequiredService<ITaskCreatedEventV2Handler>();
                            await createdEventHandler.HandleAsync(JsonSerializer.Deserialize<TaskCreatedEvent.V2>(result.Message.Value));
                            break;
                        
                        case nameof(TaskCompletedEvent):
                            var completedEventHandler = scope.ServiceProvider.GetRequiredService<ITaskCompletedEventHandler>();
                            await completedEventHandler.HandleAsync(JsonSerializer.Deserialize<TaskCompletedEvent>(result.Message.Value));
                            break;
                        
                        case nameof(TaskAssignedEvent):
                            var assignedEventHandler = scope.ServiceProvider.GetRequiredService<ITaskAssignedEventHandler>();
                            await assignedEventHandler.HandleAsync(JsonSerializer.Deserialize<TaskAssignedEvent>(result.Message.Value));
                            break;
                    }

                    Console.WriteLine($"{result.Message.Key}: {result.Message.Value}ms");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");
                    if (e.Error.IsFatal)
                        break;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
        }

        public override void Dispose()
        {
            _kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            _kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}