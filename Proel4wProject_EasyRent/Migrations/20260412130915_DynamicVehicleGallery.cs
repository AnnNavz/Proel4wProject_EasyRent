using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proel4wProject_EasyRent.Migrations
{
    /// <inheritdoc />
    public partial class DynamicVehicleGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath2",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "ImagePath3",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "ImagePath4",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "ImagePath5",
                table: "Vehicle");

            migrationBuilder.CreateTable(
                name: "VehicleImage",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleImage", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_VehicleImage_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImage_VehicleId",
                table: "VehicleImage",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleImage");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath2",
                table: "Vehicle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath3",
                table: "Vehicle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath4",
                table: "Vehicle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath5",
                table: "Vehicle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
