using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class initialDataDesign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    LastModifiedDate = table.Column<DateTime>(nullable: false),
                    LastModifiedUser = table.Column<string>(nullable: true),
                    CreateUser = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElementCoordianates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    PageNumber = table.Column<int>(nullable: false),
                    PageX = table.Column<double>(nullable: false),
                    PageY = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementCoordianates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Signatories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DocumentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signatories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signatories_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentSignatoriesLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    SignatoryId = table.Column<Guid>(nullable: false),
                    CoordinateId = table.Column<Guid>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Signatories_DocumentId",
                table: "Signatories",
                column: "DocumentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentSignatoriesLink");

            migrationBuilder.DropTable(
                name: "ElementCoordianates");

            migrationBuilder.DropTable(
                name: "Signatories");

            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
