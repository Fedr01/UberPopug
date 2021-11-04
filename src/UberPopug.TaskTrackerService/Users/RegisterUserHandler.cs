using System.Threading.Tasks;
using UberPopug.Common.Interfaces;

namespace UberPopug.TaskTrackerService.Users
{
    public class RegisterUserHandler : IKafkaHandler<string, CreateUserCommand>
    {
        private readonly TaskTrackerDbContext _context;

        public RegisterUserHandler(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(string key, CreateUserCommand value)
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