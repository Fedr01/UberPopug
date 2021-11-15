using System.Threading.Tasks;
using KafkaFlow;
using KafkaFlow.TypedHandler;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.AccountingService.Users
{
    public class UserCreatedEventHandler : IMessageHandler<UserCreatedEvent>
    {
        private readonly AccountingDbContext _dbContext;

        public UserCreatedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(IMessageContext context, UserCreatedEvent message)
        {
            _dbContext.Users.Add(new User
            {
                Email = message.Email,
                Role = message.Role
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}