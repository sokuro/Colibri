using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class removeCategoryTypesIdFromSearchEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryTypeId",
                table: "SearchEntry",
                nullable: false,
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
                onDelete: ReferentialAction.Cascade);
        }
    }
}
