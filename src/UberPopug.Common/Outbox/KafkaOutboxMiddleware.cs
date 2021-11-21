using System;
using System.IO;
using System.Threading.Tasks;
using KafkaFlow;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UberPopug.SchemaRegistry.Schemas;

namespace UberPopug.Common.Outbox
{
    public class KafkaOutboxMiddleware : IMessageMiddleware
    {
        private readonly DbContextOptionsBuilder<OutboxDbContext> _dbContextOptionsBuilder;
        private readonly ILogger<KafkaOutboxMiddleware> _logger;

        public KafkaOutboxMiddleware(IConfiguration configuration, ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<KafkaOutboxMiddleware>();

            var connectionString = configuration.GetConnectionString("Outbox");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Failed to find 'Outbox' connection string");
            }

            _dbContextOptionsBuilder = new DbContextOptionsBuilder<OutboxDbContext>();
            _dbContextOptionsBuilder.UseSqlServer(connectionString);

            var dbContext = new OutboxDbContext(_dbContextOptionsBuilder.Options);

            var createDatabaseSql = File.ReadAllText($"{AppContext.BaseDirectory}/Outbox/create-database.sql");
            dbContext.Database.ExecuteSqlRaw(createDatabaseSql);
            dbContext.Dispose();
        }


        public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                var json = JsonConvert.SerializeObject(context.Message.Value);

                var messageValue = context.Message.Value as IEvent;

                _logger.LogCritical(e,
                    $" Failed to produce {messageValue?.MetaData.EventName} {messageValue?.MetaData.Version}" +
                    $" with id {messageValue?.MetaData.EventId}" +
                    $" to {context.ProducerContext.Topic}" +
                    $" by {messageValue?.MetaData.Producer}" +
                    $" at {messageValue?.MetaData.EventTime:G}");

                var letter = new OutboxLetter(context.ProducerContext.Topic, json);

                await using var dbContext = new OutboxDbContext(_dbContextOptionsBuilder.Options);
                dbContext.Letters.Add(letter);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}