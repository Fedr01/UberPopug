using Microsoft.EntityFrameworkCore.Migrations;

namespace UberPopug.AuthService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Email);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Email", "Password", "Role" },
                values: new object[,]
                {
                    { "admin@popug.ru", "123", 3 },
                    { "manager@popug.ru", "123", 1 },
                    { "accountant@popug.ru", "123", 2 },
                    { "emp1@popug.ru", "123", 0 },
                    { "emp2@popug.ru", "123", 0 },
                    { "emp3@popug.ru", "123", 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
