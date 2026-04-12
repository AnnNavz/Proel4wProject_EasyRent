using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proel4wProject_EasyRent.Migrations
{
    /// <inheritdoc />
    public partial class VehicleSpecsAndGallery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "Performance",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Variants",
                table: "Vehicle",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Engine",
                table: "Vehicle");

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

            migrationBuilder.DropColumn(
                name: "Performance",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Variants",
                table: "Vehicle");
        }
    }
}
