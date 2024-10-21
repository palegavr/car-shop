using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarShop.CarStorage.Migrations
{
    /// <inheritdoc />
    public partial class AddCarEditProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarEditProcesses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminId = table.Column<long>(type: "bigint", nullable: false),
                    CarId = table.Column<long>(type: "bigint", nullable: false),
                    Process_Brand = table.Column<string>(type: "text", nullable: false),
                    Process_Model = table.Column<string>(type: "text", nullable: false),
                    Process_Price = table.Column<double>(type: "double precision", nullable: false),
                    Process_Color = table.Column<string>(type: "text", nullable: false),
                    Process_EngineCapacity = table.Column<double>(type: "double precision", nullable: false),
                    Process_CorpusType = table.Column<int>(type: "integer", nullable: false),
                    Process_FuelType = table.Column<int>(type: "integer", nullable: false),
                    Process_Count = table.Column<int>(type: "integer", nullable: false),
                    Process_Image = table.Column<string>(type: "text", nullable: false),
                    Process_BigImages = table.Column<string[]>(type: "text[]", nullable: false),
                    Process_AdditionalCarOptionsJson = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarEditProcesses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarEditProcesses");
        }
    }
}
