using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FabricesStoreManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnToCatalog_IsPurchase_IsPaid_Price_PaidAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPurchased",
                table: "Catalogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "Catalogs",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Catalogs",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPurchased",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Catalogs");
        }
    }
}
