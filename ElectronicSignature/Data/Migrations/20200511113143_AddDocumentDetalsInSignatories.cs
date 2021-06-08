using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class AddDocumentDetalsInSignatories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signatories_Documents_DocumentId",
                table: "Signatories");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "Signatories",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Signatories_Documents_DocumentId",
                table: "Signatories",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signatories_Documents_DocumentId",
                table: "Signatories");

            migrationBuilder.AlterColumn<Guid>(
                name: "DocumentId",
                table: "Signatories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Signatories_Documents_DocumentId",
                table: "Signatories",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
