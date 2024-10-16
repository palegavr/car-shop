using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    /// <inheritdoc />
    public partial class AddCarConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CarId = table.Column<long>(type: "bigint", nullable: false),
                    AirConditioner = table.Column<bool>(type: "boolean", nullable: false),
                    HeatedDriversSeat = table.Column<bool>(type: "boolean", nullable: false),
                    SeatHeightAdjustment = table.Column<bool>(type: "boolean", nullable: false),
                    DifferentCarColor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarConfigurations_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarConfigurations_CarId",
                table: "CarConfigurations",
                column: "CarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarConfigurations");
        }
    }
}
