using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class updateArchiveEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeOfAdvertisement",
                table: "ArchiveEntry",
                newName: "TypeOfCategoryGroup");

            migrationBuilder.AddColumn<bool>(
                name: "isOffer",
                table: "ArchiveEntry",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isOffer",
                table: "ArchiveEntry");

            migrationBuilder.RenameColumn(
                name: "TypeOfCategoryGroup",
                table: "ArchiveEntry",
                newName: "TypeOfAdvertisement");
        }
    }
}
