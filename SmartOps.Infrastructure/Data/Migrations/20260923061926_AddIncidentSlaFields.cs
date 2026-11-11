using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartOps.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIncidentSlaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ResolutionDueAt",
                table: "Incidents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RespondedAt",
                table: "Incidents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDueAt",
                table: "Incidents",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResolutionDueAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "RespondedAt",
                table: "Incidents");

            migrationBuilder.DropColumn(
                name: "ResponseDueAt",
                table: "Incidents");
        }
    }
}
