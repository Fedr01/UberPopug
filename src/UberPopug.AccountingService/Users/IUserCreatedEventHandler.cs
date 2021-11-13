using UberPopug.Common;
using UberPopug.SchemaRegistry.Schemas.Users;

namespace UberPopug.AccountingService.Users
{
    public interface IUserCreatedEventHandler : IEventHandler<UserCreatedEvent>
    {
    }
}