using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_requests_created_at",
                table: "requests",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_customers_email",
                table: "customers",
                column: "email",
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_requests_created_at",
                table: "requests");

            migrationBuilder.DropIndex(
                name: "IX_customers_email",
                table: "customers");
        }
    }
}
