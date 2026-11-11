using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartOps.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Incidents_AssignedTeamId",
                table: "Incidents",
                column: "AssignedTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_AssignedUserId",
                table: "Incidents",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Incidents_CreatedByUserId",
                table: "Incidents",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Teams_AssignedTeamId",
                table: "Incidents",
                column: "AssignedTeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Users_AssignedUserId",
                table: "Incidents",
                column: "AssignedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Incidents_Users_CreatedByUserId",
                table: "Incidents",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Teams_AssignedTeamId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Users_AssignedUserId",
                table: "Incidents");

            migrationBuilder.DropForeignKey(
                name: "FK_Incidents_Users_CreatedByUserId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_AssignedTeamId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_AssignedUserId",
                table: "Incidents");

            migrationBuilder.DropIndex(
                name: "IX_Incidents_CreatedByUserId",
                table: "Incidents");
        }
    }
}
