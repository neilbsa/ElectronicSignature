using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class removeLinkDocSigElem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentSignatoriesLink");

            migrationBuilder.AddColumn<Guid>(
                name: "DocumentId",
                table: "ElementCoordianates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SignatoryId",
                table: "ElementCoordianates",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ElementCoordianates_DocumentId",
                table: "ElementCoordianates",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ElementCoordianates_SignatoryId",
                table: "ElementCoordianates",
                column: "SignatoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElementCoordianates_Documents_DocumentId",
                table: "ElementCoordianates",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElementCoordianates_Signatories_SignatoryId",
                table: "ElementCoordianates",
                column: "SignatoryId",
                principalTable: "Signatories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElementCoordianates_Documents_DocumentId",
                table: "ElementCoordianates");

            migrationBuilder.DropForeignKey(
                name: "FK_ElementCoordianates_Signatories_SignatoryId",
                table: "ElementCoordianates");

            migrationBuilder.DropIndex(
                name: "IX_ElementCoordianates_DocumentId",
                table: "ElementCoordianates");

            migrationBuilder.DropIndex(
                name: "IX_ElementCoordianates_SignatoryId",
                table: "ElementCoordianates");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "ElementCoordianates");

            migrationBuilder.DropColumn(
                name: "SignatoryId",
                table: "ElementCoordianates");

            migrationBuilder.CreateTable(
                name: "DocumentSignatoriesLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoordinateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SignatoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentSignatoriesLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentSignatoriesLink_ElementCoordianates_CoordinateId",
                        column: x => x.CoordinateId,
                        principalTable: "ElementCoordianates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentSignatoriesLink_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentSignatoriesLink_Signatories_SignatoryId",
                        column: x => x.SignatoryId,
                        principalTable: "Signatories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSignatoriesLink_CoordinateId",
                table: "DocumentSignatoriesLink",
                column: "CoordinateId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSignatoriesLink_DocumentId",
                table: "DocumentSignatoriesLink",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentSignatoriesLink_SignatoryId",
                table: "DocumentSignatoriesLink",
                column: "SignatoryId");
        }
    }
}
