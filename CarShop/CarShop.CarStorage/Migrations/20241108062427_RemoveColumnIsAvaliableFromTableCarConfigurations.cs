using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnIsAvaliableFromTableCarConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvaliable",
                table: "CarConfigurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvaliable",
                table: "CarConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
