using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class ODA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Orders");
        }
    }
}
