using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElectronicSignature.Data.Migrations
{
    public partial class removeMigrationForOwenerEmailInFolderStruc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerEmailAddress",
                table: "FolderStructure");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "FolderStructure",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreateUser",
                table: "FolderStructure",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "FolderStructure",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedUser",
                table: "FolderStructure",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "FolderStructure");

            migrationBuilder.DropColumn(
                name: "CreateUser",
                table: "FolderStructure");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "FolderStructure");

            migrationBuilder.DropColumn(
                name: "LastModifiedUser",
                table: "FolderStructure");

            migrationBuilder.AddColumn<string>(
                name: "OwnerEmailAddress",
                table: "FolderStructure",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
