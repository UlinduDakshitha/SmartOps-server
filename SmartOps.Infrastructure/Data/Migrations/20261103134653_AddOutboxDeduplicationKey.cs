using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartOps.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxDeduplicationKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeduplicationKey",
                table: "AutomationOutbox",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AutomationOutbox_DeduplicationKey",
                table: "AutomationOutbox",
                column: "DeduplicationKey",
                unique: true,
                filter: "\"DeduplicationKey\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AutomationOutbox_DeduplicationKey",
                table: "AutomationOutbox");

            migrationBuilder.DropColumn(
                name: "DeduplicationKey",
                table: "AutomationOutbox");
        }
    }
}
