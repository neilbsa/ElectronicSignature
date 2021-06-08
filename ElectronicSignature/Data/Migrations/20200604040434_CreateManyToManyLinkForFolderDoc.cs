using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class CreateManyToManyLinkForFolderDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "Documents");

            migrationBuilder.CreateTable(
                name: "FolderStrucDocumentLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    FolderStrucId = table.Column<Guid>(nullable: false),
                    FolderStrucDetailId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderStrucDocumentLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderStrucDocumentLink_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderStrucDocumentLink_FolderStructure_FolderStrucDetailId",
                        column: x => x.FolderStrucDetailId,
                        principalTable: "FolderStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolderStrucDocumentLink_DocumentId",
                table: "FolderStrucDocumentLink",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderStrucDocumentLink_FolderStrucDetailId",
                table: "FolderStrucDocumentLink",
                column: "FolderStrucDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderStrucDocumentLink");

            migrationBuilder.AddColumn<Guid>(
                name: "FolderId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
