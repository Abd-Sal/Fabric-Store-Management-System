using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FabricesStoreManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class remove_customer_catalog_index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CatalogsAssigns_CatalogID_CustomerID",
                table: "CatalogsAssigns");

            migrationBuilder.DropIndex(
                name: "IX_CatalogsAssigns_CatalogID",
                table: "CatalogsAssigns");

            migrationBuilder.DropIndex(
                name: "IX_CatalogsAssigns_CustomerID",
                table: "CatalogsAssigns");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CatalogsAssigns_CatalogID_CustomerID",
                table: "CatalogsAssigns",
                columns: new[] { "CatalogID", "CustomerID" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogsAssigns_CatalogID",
                table: "CatalogsAssigns",
                columns: new[] { "CatalogID" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogsAssigns_CustomerID",
                table: "CatalogsAssigns",
                columns: new[] { "CustomerID" });
        }
    }
}
