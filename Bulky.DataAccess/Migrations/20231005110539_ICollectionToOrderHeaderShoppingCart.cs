using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class ICollectionToOrderHeaderShoppingCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderHeaderId",
                table: "AddressNew",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId",
                table: "AddressNew",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressNew_OrderHeaderId",
                table: "AddressNew",
                column: "OrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressNew_ShoppingCartId",
                table: "AddressNew",
                column: "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressNew_OrderHeaders_OrderHeaderId",
                table: "AddressNew",
                column: "OrderHeaderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressNew_ShoppingCart_ShoppingCartId",
                table: "AddressNew",
                column: "ShoppingCartId",
                principalTable: "ShoppingCart",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressNew_OrderHeaders_OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.DropForeignKey(
                name: "FK_AddressNew_ShoppingCart_ShoppingCartId",
                table: "AddressNew");

            migrationBuilder.DropIndex(
                name: "IX_AddressNew_OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.DropIndex(
                name: "IX_AddressNew_ShoppingCartId",
                table: "AddressNew");

            migrationBuilder.DropColumn(
                name: "OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId",
                table: "AddressNew");
        }
    }
}
