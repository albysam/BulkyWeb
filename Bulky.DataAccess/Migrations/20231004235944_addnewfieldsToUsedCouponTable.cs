using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkyWeb.Migrations
{
    /// <inheritdoc />
    public partial class addnewfieldsToUsedCouponTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Discount",
                table: "AppliedCoupon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MaxCartAmount",
                table: "AppliedCoupon",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinCartAmount",
                table: "AppliedCoupon",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "AppliedCoupon");

            migrationBuilder.DropColumn(
                name: "MaxCartAmount",
                table: "AppliedCoupon");

            migrationBuilder.DropColumn(
                name: "MinCartAmount",
                table: "AppliedCoupon");
        }
    }
}
