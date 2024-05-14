using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlissShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsAprovedForShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAproved",
                table: "Shops",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAproved",
                table: "Shops");
        }
    }
}
