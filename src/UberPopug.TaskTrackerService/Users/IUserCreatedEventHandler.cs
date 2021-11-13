using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.TaskTrackerService.Users
{
    public interface IUserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
    }
}