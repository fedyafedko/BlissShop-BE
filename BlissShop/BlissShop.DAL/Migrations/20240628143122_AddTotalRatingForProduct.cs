using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlissShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalRatingForProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalRating",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRating",
                table: "Products");
        }
    }
}
