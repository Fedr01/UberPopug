using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.AccountingService.Users
{
    public class UserCreatedEventHandler : IUserCreatedEventHandler
    {
        private readonly AccountingDbContext _dbContext;

        public UserCreatedEventHandler(AccountingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(UserCreatedEvent @event)
        {
            _dbContext.Users.Add(new User
            {
                Email = @event.Email,
                Role = @event.Role
            });

            await _dbContext.SaveChangesAsync();
        }
    }
}