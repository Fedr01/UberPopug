using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UberPopug.Common.Constants;
using UberPopug.SchemaRegistry;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.AccountingService.Users
{
    public class UsersStreamConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _kafkaConsumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _topic;

        public UsersStreamConsumer(ConsumerConfig config, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _topic = KafkaTopics.UsersStream;
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
                        case nameof(UserCreatedEvent):
                            var createdEventHandler = scope.ServiceProvider.GetRequiredService<IUserCreatedEventHandler>();
                            await createdEventHandler.HandleAsync(JsonSerializer.Deserialize<UserCreatedEvent>(result.Message.Value));
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