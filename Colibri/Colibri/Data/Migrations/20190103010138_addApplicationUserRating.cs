using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addApplicationUserRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUserRatings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApplicationUserRatedName = table.Column<string>(nullable: true),
                    ApplicationUserRatedId = table.Column<string>(nullable: true),
                    ApplicationUserRatingId = table.Column<string>(nullable: true),
                    ApplicationUserRatingName = table.Column<string>(nullable: true),
                    ApplicationUserRate = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRatings_AspNetUsers_ApplicationUserRatedId",
                        column: x => x.ApplicationUserRatedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationUserRatings_AspNetUsers_ApplicationUserRatingId",
                        column: x => x.ApplicationUserRatingId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRatings_ApplicationUserRatedId",
                table: "ApplicationUserRatings",
                column: "ApplicationUserRatedId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserRatings_ApplicationUserRatingId",
                table: "ApplicationUserRatings",
                column: "ApplicationUserRatingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserRatings");
        }
    }
}
