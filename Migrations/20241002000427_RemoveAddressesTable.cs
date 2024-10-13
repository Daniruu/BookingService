using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAddressesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "Address_BuildingNumber",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_PostalCode",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Region",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_RoomNumber",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Businesses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_BuildingNumber",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Address_City",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Address_PostalCode",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Address_Region",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Address_RoomNumber",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Businesses");

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    BuildingNumber = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    Region = table.Column<string>(type: "text", nullable: false),
                    RoomNumber = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.BusinessId);
                    table.ForeignKey(
                        name: "FK_Addresses_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
