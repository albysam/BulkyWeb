using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class addWalletTotalToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddressNew_OrderHeaders_OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.DropIndex(
                name: "IX_AddressNew_OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.DropColumn(
                name: "OrderHeaderId",
                table: "AddressNew");

            migrationBuilder.CreateTable(
                name: "WalletTotal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletBalance = table.Column<double>(type: "float", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTotal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTotal_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTotal_ApplicationUserId",
                table: "WalletTotal",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTotal");

            migrationBuilder.AddColumn<int>(
                name: "OrderHeaderId",
                table: "AddressNew",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AddressNew_OrderHeaderId",
                table: "AddressNew",
                column: "OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddressNew_OrderHeaders_OrderHeaderId",
                table: "AddressNew",
                column: "OrderHeaderId",
                principalTable: "OrderHeaders",
                principalColumn: "Id");
        }
    }
}
