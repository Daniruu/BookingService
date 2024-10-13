using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class AddNipRegonAndKrsToBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Krs",
                table: "Businesses",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nip",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Regon",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Krs",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Nip",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Regon",
                table: "Businesses");
        }
    }
}
