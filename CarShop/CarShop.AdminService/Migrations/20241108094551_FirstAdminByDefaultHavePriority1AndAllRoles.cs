using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.AdminService.Migrations
{
    /// <inheritdoc />
    public partial class FirstAdminByDefaultHavePriority1AndAllRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Priority", "Roles" },
                values: new object[] { 1, new[] { "*" } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "Priority", "Roles" },
                values: new object[] { 1000, new string[0] });
        }
    }
}
