using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyAndDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_products_ProductId",
                table: "RequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestItems_requests_RequestId",
                table: "RequestItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestItems",
                table: "RequestItems");

            migrationBuilder.RenameTable(
                name: "RequestItems",
                newName: "request_items");

            migrationBuilder.RenameColumn(
                name: "quoted_price",
                table: "product_price_histories",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "request_items",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "request_items",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "request_items",
                newName: "updated_by");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "request_items",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "request_items",
                newName: "unit_price");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "request_items",
                newName: "request_id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "request_items",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "LineTotal",
                table: "request_items",
                newName: "line_total");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "request_items",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "request_items",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "request_items",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_RequestItems_RequestId",
                table: "request_items",
                newName: "IX_request_items_request_id");

            migrationBuilder.RenameIndex(
                name: "IX_RequestItems_ProductId",
                table: "request_items",
                newName: "IX_request_items_product_id");

            migrationBuilder.AddColumn<int>(
                name: "currency",
                table: "requests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "last_request_currency",
                table: "products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "currency",
                table: "product_price_histories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "unit_price",
                table: "request_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "line_total",
                table: "request_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<bool>(
                name: "is_deleted",
                table: "request_items",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                table: "request_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_request_items",
                table: "request_items",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_request_items_products_product_id",
                table: "request_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_request_items_requests_request_id",
                table: "request_items",
                column: "request_id",
                principalTable: "requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_request_items_products_product_id",
                table: "request_items");

            migrationBuilder.DropForeignKey(
                name: "FK_request_items_requests_request_id",
                table: "request_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_request_items",
                table: "request_items");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "requests");

            migrationBuilder.DropColumn(
                name: "last_request_currency",
                table: "products");

            migrationBuilder.DropColumn(
                name: "currency",
                table: "product_price_histories");

            migrationBuilder.DropColumn(
                name: "discount",
                table: "request_items");

            migrationBuilder.RenameTable(
                name: "request_items",
                newName: "RequestItems");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "product_price_histories",
                newName: "quoted_price");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "RequestItems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RequestItems",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "RequestItems",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "RequestItems",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "RequestItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "request_id",
                table: "RequestItems",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "RequestItems",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "line_total",
                table: "RequestItems",
                newName: "LineTotal");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "RequestItems",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "RequestItems",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "RequestItems",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_request_items_request_id",
                table: "RequestItems",
                newName: "IX_RequestItems_RequestId");

            migrationBuilder.RenameIndex(
                name: "IX_request_items_product_id",
                table: "RequestItems",
                newName: "IX_RequestItems_ProductId");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "RequestItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "LineTotal",
                table: "RequestItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "RequestItems",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestItems",
                table: "RequestItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_products_ProductId",
                table: "RequestItems",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestItems_requests_RequestId",
                table: "RequestItems",
                column: "RequestId",
                principalTable: "requests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
