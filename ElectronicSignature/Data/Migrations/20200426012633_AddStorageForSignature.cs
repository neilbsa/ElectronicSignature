using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class AddStorageForSignature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "base64signature",
                table: "ElementCoordianates",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SignatureTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SignatoryId = table.Column<Guid>(nullable: false),
                    base64stringFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatureTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatureTemplates_Signatories_SignatoryId",
                        column: x => x.SignatoryId,
                        principalTable: "Signatories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignatureTemplates_SignatoryId",
                table: "SignatureTemplates",
                column: "SignatoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignatureTemplates");

            migrationBuilder.DropColumn(
                name: "base64signature",
                table: "ElementCoordianates");
        }
    }
}
