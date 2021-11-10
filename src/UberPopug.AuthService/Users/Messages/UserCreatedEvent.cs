using UberPopug.Common;

namespace UberPopug.AuthService.Users.Messages
{
    public class UserCreatedEvent : Event
    {
        public UserCreatedEvent(string email, Role role) : base(nameof(UserCreatedEvent))
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