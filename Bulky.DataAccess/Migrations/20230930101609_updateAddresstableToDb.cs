using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class updateAddresstableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "user_Id",
                table: "Addresses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_user_Id",
                table: "Addresses",
                column: "user_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_user_Id",
                table: "Addresses",
                column: "user_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_user_Id",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_user_Id",
                table: "Addresses");

            migrationBuilder.AlterColumn<string>(
                name: "user_Id",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
