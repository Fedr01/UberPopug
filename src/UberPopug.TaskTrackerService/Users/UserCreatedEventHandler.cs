using System.Threading.Tasks;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.TaskTrackerService.Users
{
    public class UserCreatedEventHandler : IUserCreatedEventHandler
    {
        private readonly TaskTrackerDbContext _dbContext;

        public UserCreatedEventHandler(TaskTrackerDbContext dbContext)
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