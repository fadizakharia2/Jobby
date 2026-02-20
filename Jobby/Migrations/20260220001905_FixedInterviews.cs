using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobby.Migrations
{
    /// <inheritdoc />
    public partial class FixedInterviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                schema: "jobby",
                table: "interviews",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                schema: "jobby",
                table: "interviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                schema: "jobby",
                table: "interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "jobby",
                table: "interviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                schema: "jobby",
                table: "interviews",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelReason",
                schema: "jobby",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "jobby",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                schema: "jobby",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "jobby",
                table: "interviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "jobby",
                table: "interviews");
        }
    }
}
