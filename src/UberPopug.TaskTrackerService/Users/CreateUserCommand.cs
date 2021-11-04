using UberPopug.Common;

namespace UberPopug.TaskTrackerService.Users
{
    public class CreateUserCommand : Command
    {
        public CreateUserCommand(string email, Role role) : base("users")
        {
            Email = email;
            Role = role;
        }

        public string Email { get; private set; }

        public Role Role { get; private set; }
    }

    public class UserUpdatedCommand
    {
        public string Email { get; set; }

        public Role Role { get; set; }
    }

    public class UserDeletedCommand
    {
        public string Email { get; set; }

        public Role Role { get; set; }
    }
}