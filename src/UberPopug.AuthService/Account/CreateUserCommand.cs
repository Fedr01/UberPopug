using UberPopug.AuthService.Users;

namespace UberPopug.AuthService.Account
{
    public class CreateUserCommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; } = Role.Employee;
    }
}