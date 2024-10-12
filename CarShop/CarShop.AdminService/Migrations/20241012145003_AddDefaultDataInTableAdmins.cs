using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.AdminService.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultDataInTableAdmins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Admins",
                columns: new[] { "Id", "Banned", "Email", "Password" },
                values: new object[] { 1L, false, "admin@admin.com", "$argon2id$v=19$m=65536,t=3,p=1$iICM/5uHlAHETRq8PtSHxg$jnk1HHpTP4voBpY80g5LCciaToO9WNT4X4IM7FL2KKk" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Admins",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
