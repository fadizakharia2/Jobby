using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobby.Migrations
{
    /// <inheritdoc />
    public partial class AddedFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "file",
                schema: "jobby",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StorageKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OriginalName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_file", x => x.Id);
                    table.ForeignKey(
                        name: "FK_file_users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalSchema: "jobby",
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "application_files",
                schema: "jobby",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilePurpose = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_files", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_application_files_applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "jobby",
                        principalTable: "applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_application_files_file_FileId",
                        column: x => x.FileId,
                        principalSchema: "jobby",
                        principalTable: "file",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_files_ApplicationId_FileId",
                schema: "jobby",
                table: "application_files",
                columns: new[] { "ApplicationId", "FileId" });

            migrationBuilder.CreateIndex(
                name: "IX_file_Id",
                schema: "jobby",
                table: "file",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_file_OwnerUserId",
                schema: "jobby",
                table: "file",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_files",
                schema: "jobby");

            migrationBuilder.DropTable(
                name: "file",
                schema: "jobby");
        }
    }
}
