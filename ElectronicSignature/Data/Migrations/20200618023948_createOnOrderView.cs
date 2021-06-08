using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class createOnOrderView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStrucDocumentLink_FolderStructure_FolderStrucDetailId",
                table: "FolderStrucDocumentLink");

            migrationBuilder.DropIndex(
                name: "IX_FolderStrucDocumentLink_FolderStrucDetailId",
                table: "FolderStrucDocumentLink");

            migrationBuilder.DropColumn(
                name: "FolderStrucDetailId",
                table: "FolderStrucDocumentLink");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "Signatories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsScheduledDelivery",
                table: "Documents",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_FolderStrucDocumentLink_FolderStrucId",
                table: "FolderStrucDocumentLink",
                column: "FolderStrucId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStrucDocumentLink_FolderStructure_FolderStrucId",
                table: "FolderStrucDocumentLink",
                column: "FolderStrucId",
                principalTable: "FolderStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FolderStrucDocumentLink_FolderStructure_FolderStrucId",
                table: "FolderStrucDocumentLink");

            migrationBuilder.DropIndex(
                name: "IX_FolderStrucDocumentLink_FolderStrucId",
                table: "FolderStrucDocumentLink");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Signatories");

            migrationBuilder.DropColumn(
                name: "IsScheduledDelivery",
                table: "Documents");

            migrationBuilder.AddColumn<Guid>(
                name: "FolderStrucDetailId",
                table: "FolderStrucDocumentLink",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolderStrucDocumentLink_FolderStrucDetailId",
                table: "FolderStrucDocumentLink",
                column: "FolderStrucDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_FolderStrucDocumentLink_FolderStructure_FolderStrucDetailId",
                table: "FolderStrucDocumentLink",
                column: "FolderStrucDetailId",
                principalTable: "FolderStructure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
