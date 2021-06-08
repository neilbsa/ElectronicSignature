using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class MakeMotherFolderIdnullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStructure_FolderStructure_MotherFolderId",
                table: "FolderStructure");

            migrationBuilder.AlterColumn<Guid>(
                name: "MotherFolderId",
                table: "FolderStructure",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "MotherFolderId",
                table: "FolderStructure",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStructure_FolderStructure_MotherFolderId",
                table: "FolderStructure",
                column: "MotherFolderId",
                principalTable: "FolderStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
