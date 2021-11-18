namespace UberPopug.SchemaRegistry.Schemas.Users
{
    public class UserCreatedEvent : IEvent
    {
        public UserCreatedEvent(string email, string role)
        {
            Email = email;
            Role = role;
        }

        public UserCreatedEvent()
        {
        }

        public string Email { get; set; }
        public string Role { get; set; }

        public EventMetaData MetaData { get; set; } = new EventMetaData(nameof(UserCreatedEvent), "v1");
    }
}