using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addCategoryGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryGroupId",
                table: "Products",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoryGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryGroups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryGroupId",
                table: "Products",
                column: "CategoryGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CategoryGroups_CategoryGroupId",
                table: "Products",
                column: "CategoryGroupId",
                principalTable: "CategoryGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CategoryGroups_CategoryGroupId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "CategoryGroups");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryGroupId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryGroupId",
                table: "Products");
        }
    }
}
