using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addCategoryTypePropertyToNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryTypeId",
                table: "Notifications",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CategoryTypeId",
                table: "Notifications",
                column: "CategoryTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CategoryTypes_CategoryTypeId",
                table: "Notifications",
                column: "CategoryTypeId",
                principalTable: "CategoryTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CategoryTypes_CategoryTypeId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CategoryTypeId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CategoryTypeId",
                table: "Notifications");
        }
    }
}
