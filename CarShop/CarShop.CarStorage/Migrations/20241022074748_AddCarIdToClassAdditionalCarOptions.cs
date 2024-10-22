using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    /// <inheritdoc />
    public partial class AddCarIdToClassAdditionalCarOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCarOption_Cars_CarId",
                table: "AdditionalCarOption");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalCarOption",
                table: "AdditionalCarOption");

            migrationBuilder.RenameTable(
                name: "AdditionalCarOption",
                newName: "AdditionalCarOptions");

            migrationBuilder.RenameIndex(
                name: "IX_AdditionalCarOption_CarId",
                table: "AdditionalCarOptions",
                newName: "IX_AdditionalCarOptions_CarId");

            migrationBuilder.AlterColumn<long>(
                name: "CarId",
                table: "AdditionalCarOptions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalCarOptions",
                table: "AdditionalCarOptions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCarOptions_Cars_CarId",
                table: "AdditionalCarOptions",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCarOptions_Cars_CarId",
                table: "AdditionalCarOptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdditionalCarOptions",
                table: "AdditionalCarOptions");

            migrationBuilder.RenameTable(
                name: "AdditionalCarOptions",
                newName: "AdditionalCarOption");

            migrationBuilder.RenameIndex(
                name: "IX_AdditionalCarOptions_CarId",
                table: "AdditionalCarOption",
                newName: "IX_AdditionalCarOption_CarId");

            migrationBuilder.AlterColumn<long>(
                name: "CarId",
                table: "AdditionalCarOption",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdditionalCarOption",
                table: "AdditionalCarOption",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCarOption_Cars_CarId",
                table: "AdditionalCarOption",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");
        }
    }
}
