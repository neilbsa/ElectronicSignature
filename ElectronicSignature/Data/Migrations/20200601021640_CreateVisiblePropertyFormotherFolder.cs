using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class CreateVisiblePropertyFormotherFolder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStructure_FolderStructure_FolderStructureModelId",
                table: "FolderStructure");

            migrationBuilder.DropIndex(
                name: "IX_FolderStructure_FolderStructureModelId",
                table: "FolderStructure");

            migrationBuilder.DropColumn(
                name: "FolderStructureModelId",
                table: "FolderStructure");

            migrationBuilder.AddColumn<Guid>(
                name: "MotherFolderId",
                table: "FolderStructure",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FolderStructure_MotherFolderId",
                table: "FolderStructure",
                column: "MotherFolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStructure_FolderStructure_MotherFolderId",
                table: "FolderStructure",
                column: "MotherFolderId",
                principalTable: "FolderStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStructure_FolderStructure_MotherFolderId",
                table: "FolderStructure");

            migrationBuilder.DropIndex(
                name: "IX_FolderStructure_MotherFolderId",
                table: "FolderStructure");

            migrationBuilder.DropColumn(
                name: "MotherFolderId",
                table: "FolderStructure");

            migrationBuilder.AddColumn<Guid>(
                name: "FolderStructureModelId",
                table: "FolderStructure",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolderStructure_FolderStructureModelId",
                table: "FolderStructure",
                column: "FolderStructureModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStructure_FolderStructure_FolderStructureModelId",
                table: "FolderStructure",
                column: "FolderStructureModelId",
                principalTable: "FolderStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
