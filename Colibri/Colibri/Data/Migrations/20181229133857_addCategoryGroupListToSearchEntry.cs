using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addCategoryGroupListToSearchEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
