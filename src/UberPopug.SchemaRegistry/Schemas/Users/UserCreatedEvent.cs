namespace UberPopug.SchemaRegistry.Schemas.Users
{
    public class UserCreatedEvent : Event
    {
        public UserCreatedEvent(string email, string role) : base(nameof(UserCreatedEvent), "v1")
        {
            Email = email;
            Role = role;
        }

        public string Email { get; private set; }

        public string Role { get; private set; }
    }
}