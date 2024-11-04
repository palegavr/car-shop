using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleteForCarEditProcessWhenDeleteCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarEditProcesses_CarId",
                table: "CarEditProcesses",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarEditProcesses_Cars_CarId",
                table: "CarEditProcesses",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarEditProcesses_Cars_CarId",
                table: "CarEditProcesses");

            migrationBuilder.DropIndex(
                name: "IX_CarEditProcesses_CarId",
                table: "CarEditProcesses");
        }
    }
}
