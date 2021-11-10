using System.Threading.Tasks;
using UberPopug.Common.Interfaces;

namespace UberPopug.TaskTrackerService.Users.Messages
{
    public class UsersStreamHandler : IKafkaHandler<string, UserCreatedEvent>
    {
        private readonly TaskTrackerDbContext _context;

        public UsersStreamHandler(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(string key, UserCreatedEvent value)
        {
            _context.Users.Add(new User
            {
                Email = value.Email,
                Role = value.Role
            });

            await _context.SaveChangesAsync();
        }
    }
}