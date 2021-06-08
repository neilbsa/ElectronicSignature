using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class createFolderStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolderStructure",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    text = table.Column<string>(nullable: true),
                    OwnerEmailAddress = table.Column<string>(nullable: true),
                    FolderStructureModelId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderStructure", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderStructure_FolderStructure_FolderStructureModelId",
                        column: x => x.FolderStructureModelId,
                        principalTable: "FolderStructure",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolderStructure_FolderStructureModelId",
                table: "FolderStructure",
                column: "FolderStructureModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderStructure");
        }
    }
}
