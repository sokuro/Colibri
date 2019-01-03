using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addSearchEntryToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameCombined",
                table: "CategoryTypes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SearchEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SearchText = table.Column<string>(nullable: true),
                    SearchDate = table.Column<DateTime>(nullable: false),
                    FullSuccess = table.Column<bool>(nullable: false),
                    PartSuccess = table.Column<bool>(nullable: false),
                    NoSuccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchEntry", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchEntry");

            migrationBuilder.DropColumn(
                name: "NameCombined",
                table: "CategoryTypes");
        }
    }
}
