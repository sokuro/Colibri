using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addArchiveEntryToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryGroupId",
                table: "CategoryTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CategoryTypes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CategoryTypes",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PLZ",
                table: "CategoryTypes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "CategoryTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isGlobal",
                table: "CategoryTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "CategoryGroups",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ArchiveEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    TypeOfAdvertisement = table.Column<string>(nullable: false),
                    CategoryTypesId = table.Column<int>(nullable: true),
                    CategoryGroupsId = table.Column<int>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchiveEntry_CategoryGroups_CategoryGroupsId",
                        column: x => x.CategoryGroupsId,
                        principalTable: "CategoryGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ArchiveEntry_CategoryTypes_CategoryTypesId",
                        column: x => x.CategoryTypesId,
                        principalTable: "CategoryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryTypes_CategoryGroupId",
                table: "CategoryTypes",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryGroupsId",
                table: "ArchiveEntry",
                column: "CategoryGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryTypesId",
                table: "ArchiveEntry",
                column: "CategoryTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryTypes_CategoryGroups_CategoryGroupId",
                table: "CategoryTypes",
                column: "CategoryGroupId",
                principalTable: "CategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryTypes_CategoryGroups_CategoryGroupId",
                table: "CategoryTypes");

            migrationBuilder.DropTable(
                name: "ArchiveEntry");

            migrationBuilder.DropIndex(
                name: "IX_CategoryTypes_CategoryGroupId",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "CategoryGroupId",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "PLZ",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "isGlobal",
                table: "CategoryTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "CategoryGroups");
        }
    }
}
