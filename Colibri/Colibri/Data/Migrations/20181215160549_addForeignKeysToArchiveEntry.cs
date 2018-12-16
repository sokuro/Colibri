using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addForeignKeysToArchiveEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveEntry_CategoryGroups_CategoryGroupsId",
                table: "ArchiveEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveEntry_CategoryTypes_CategoryTypesId",
                table: "ArchiveEntry");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveEntry_CategoryGroupsId",
                table: "ArchiveEntry");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveEntry_CategoryTypesId",
                table: "ArchiveEntry");

            migrationBuilder.DropColumn(
                name: "CategoryGroupsId",
                table: "ArchiveEntry");

            migrationBuilder.DropColumn(
                name: "CategoryTypesId",
                table: "ArchiveEntry");

            migrationBuilder.AddColumn<int>(
                name: "CategoryGroupId",
                table: "ArchiveEntry",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryTypeId",
                table: "ArchiveEntry",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryGroupId",
                table: "ArchiveEntry",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryTypeId",
                table: "ArchiveEntry",
                column: "CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchiveEntry_CategoryGroups_CategoryGroupId",
                table: "ArchiveEntry",
                column: "CategoryGroupId",
                principalTable: "CategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchiveEntry_CategoryTypes_CategoryTypeId",
                table: "ArchiveEntry",
                column: "CategoryTypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveEntry_CategoryGroups_CategoryGroupId",
                table: "ArchiveEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_ArchiveEntry_CategoryTypes_CategoryTypeId",
                table: "ArchiveEntry");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveEntry_CategoryGroupId",
                table: "ArchiveEntry");

            migrationBuilder.DropIndex(
                name: "IX_ArchiveEntry_CategoryTypeId",
                table: "ArchiveEntry");

            migrationBuilder.DropColumn(
                name: "CategoryGroupId",
                table: "ArchiveEntry");

            migrationBuilder.DropColumn(
                name: "CategoryTypeId",
                table: "ArchiveEntry");

            migrationBuilder.AddColumn<int>(
                name: "CategoryGroupsId",
                table: "ArchiveEntry",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryTypesId",
                table: "ArchiveEntry",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryGroupsId",
                table: "ArchiveEntry",
                column: "CategoryGroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveEntry_CategoryTypesId",
                table: "ArchiveEntry",
                column: "CategoryTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchiveEntry_CategoryGroups_CategoryGroupsId",
                table: "ArchiveEntry",
                column: "CategoryGroupsId",
                principalTable: "CategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ArchiveEntry_CategoryTypes_CategoryTypesId",
                table: "ArchiveEntry",
                column: "CategoryTypesId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
