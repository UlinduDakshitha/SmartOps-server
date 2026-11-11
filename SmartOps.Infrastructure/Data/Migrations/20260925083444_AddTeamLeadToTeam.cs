using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartOps.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamLeadToTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeamLeadId",
                table: "Teams",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_TeamLeadId",
                table: "Teams",
                column: "TeamLeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Users_TeamLeadId",
                table: "Teams",
                column: "TeamLeadId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Users_TeamLeadId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_TeamLeadId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "TeamLeadId",
                table: "Teams");
        }
    }
}
