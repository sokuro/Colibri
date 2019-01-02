using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addCategoryTypeToSearchEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryGroups_SearchEntry_SearchEntryId",
                table: "CategoryGroups");

            migrationBuilder.DropIndex(
                name: "IX_CategoryGroups_SearchEntryId",
                table: "CategoryGroups");

            migrationBuilder.DropColumn(
                name: "SearchEntryId",
                table: "CategoryGroups");

            migrationBuilder.AddColumn<int>(
                name: "CategoryTypeId",
                table: "SearchEntry",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SearchEntry_CategoryTypeId",
                table: "SearchEntry",
                column: "CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchEntry_CategoryTypes_CategoryTypeId",
                table: "SearchEntry",
                column: "CategoryTypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchEntry_CategoryTypes_CategoryTypeId",
                table: "SearchEntry");

            migrationBuilder.DropIndex(
                name: "IX_SearchEntry_CategoryTypeId",
                table: "SearchEntry");

            migrationBuilder.DropColumn(
                name: "CategoryTypeId",
                table: "SearchEntry");

            migrationBuilder.AddColumn<int>(
                name: "SearchEntryId",
                table: "CategoryGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryGroups_SearchEntryId",
                table: "CategoryGroups",
                column: "SearchEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryGroups_SearchEntry_SearchEntryId",
                table: "CategoryGroups",
                column: "SearchEntryId",
                principalTable: "SearchEntry",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
