using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class removeSpecialTagFromUserService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserServices_SpecialTags_SpecialTagId",
                table: "UserServices");

            migrationBuilder.DropIndex(
                name: "IX_UserServices_SpecialTagId",
                table: "UserServices");

            migrationBuilder.DropColumn(
                name: "SpecialTagId",
                table: "UserServices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialTagId",
                table: "UserServices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_SpecialTagId",
                table: "UserServices",
                column: "SpecialTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserServices_SpecialTags_SpecialTagId",
                table: "UserServices",
                column: "SpecialTagId",
                principalTable: "SpecialTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
