using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.AdminService.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndPriorityInTableAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Admins",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string[]>(
                name: "Roles",
                table: "Admins",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
            
            migrationBuilder.Sql(@"
                UPDATE ""Admins"" 
                SET ""Priority"" = 1000, 
                    ""Roles"" = ARRAY[
                        'admin.account.change-password.own',
                        'admin.account.ban.own'
                    ]::text[]
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Admins");
        }
    }
}
