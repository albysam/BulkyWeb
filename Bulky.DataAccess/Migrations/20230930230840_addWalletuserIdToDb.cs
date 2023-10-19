using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class addWalletuserIdToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_userId",
                table: "Wallet",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_AspNetUsers_userId",
                table: "Wallet",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_AspNetUsers_userId",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_userId",
                table: "Wallet");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Wallet",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
