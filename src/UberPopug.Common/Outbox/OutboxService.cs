using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using KafkaFlow.Producers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using UberPopug.SchemaRegistry.Schemas;

namespace UberPopug.Common.Outbox
{
    public class OutboxService : BackgroundService
    {
        private readonly DbContextOptionsBuilder<OutboxDbContext> _dbContextOptionsBuilder;
        private readonly IProducerAccessor _producerAccessor;

        public OutboxService(IConfiguration configuration, IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor;
            var connectionString = configuration.GetConnectionString("Outbox");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Failed to find 'Outbox' connection string");
            }

            _dbContextOptionsBuilder = new DbContextOptionsBuilder<OutboxDbContext>();
            _dbContextOptionsBuilder.UseSqlServer(connectionString);

            using var dbContext = new OutboxDbContext(_dbContextOptionsBuilder.Options);
            var createDatabaseSql = File.ReadAllText($"{AppContext.BaseDirectory}/Outbox/create-database.sql");
            dbContext.Database.ExecuteSqlRaw(createDatabaseSql);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var task = Task.Run(async () => await ProcessQueue(stoppingToken), stoppingToken);
        }

        private async Task ProcessQueue(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await using var dbContext = new OutboxDbContext(_dbContextOptionsBuilder.Options);
                var letters = await dbContext.Letters.ToListAsync(stoppingToken);

                foreach (var outboxLetter in letters)
                {
                    var evnt = JsonConvert.DeserializeObject<Event>(outboxLetter.Body);
                    var typeName = evnt.MetaData.EventName;

                    var type = Assembly.GetAssembly(typeof(IEvent)).GetTypes().FirstOrDefault(t => t.Name == typeName);
                    var typedMessage = JsonConvert.DeserializeObject(outboxLetter.Body, type);

                    var producer = _producerAccessor.GetProducer(type.FullName);
                    await producer.ProduceAsync(evnt.MetaData.EventId.ToString(), typedMessage);
                }
            }
        }
    }
}