using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlissShop.DAL.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCart_AspNetUsers_UserId",
                table: "ProductCart");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCartItem_ProductCart_ProductCartId",
                table: "ProductCartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCartItem_Products_ProductId",
                table: "ProductCartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCartItem",
                table: "ProductCartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCart",
                table: "ProductCart");

            migrationBuilder.RenameTable(
                name: "ProductCartItem",
                newName: "ProductCartItems");

            migrationBuilder.RenameTable(
                name: "ProductCart",
                newName: "ProductCarts");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCartItem_ProductId",
                table: "ProductCartItems",
                newName: "IX_ProductCartItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCartItem_ProductCartId",
                table: "ProductCartItems",
                newName: "IX_ProductCartItems_ProductCartId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCart_UserId",
                table: "ProductCarts",
                newName: "IX_ProductCarts_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCartItems",
                table: "ProductCartItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCarts",
                table: "ProductCarts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCartItems_ProductCarts_ProductCartId",
                table: "ProductCartItems",
                column: "ProductCartId",
                principalTable: "ProductCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCartItems_Products_ProductId",
                table: "ProductCartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCarts_AspNetUsers_UserId",
                table: "ProductCarts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCartItems_ProductCarts_ProductCartId",
                table: "ProductCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCartItems_Products_ProductId",
                table: "ProductCartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCarts_AspNetUsers_UserId",
                table: "ProductCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCarts",
                table: "ProductCarts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCartItems",
                table: "ProductCartItems");

            migrationBuilder.RenameTable(
                name: "ProductCarts",
                newName: "ProductCart");

            migrationBuilder.RenameTable(
                name: "ProductCartItems",
                newName: "ProductCartItem");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCarts_UserId",
                table: "ProductCart",
                newName: "IX_ProductCart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCartItems_ProductId",
                table: "ProductCartItem",
                newName: "IX_ProductCartItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCartItems_ProductCartId",
                table: "ProductCartItem",
                newName: "IX_ProductCartItem_ProductCartId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCart",
                table: "ProductCart",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCartItem",
                table: "ProductCartItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCart_AspNetUsers_UserId",
                table: "ProductCart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCartItem_ProductCart_ProductCartId",
                table: "ProductCartItem",
                column: "ProductCartId",
                principalTable: "ProductCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCartItem_Products_ProductId",
                table: "ProductCartItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
