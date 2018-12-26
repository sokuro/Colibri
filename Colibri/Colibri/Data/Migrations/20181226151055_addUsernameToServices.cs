using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addUsernameToServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameCombined",
                table: "CategoryTypes");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserName",
                table: "UserServices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserName",
                table: "UserServices");

            migrationBuilder.AddColumn<string>(
                name: "NameCombined",
                table: "CategoryTypes",
                nullable: true);
        }
    }
}
