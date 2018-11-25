using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addUserServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    Available = table.Column<bool>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Duration = table.Column<double>(nullable: false),
                    NumberOfClicks = table.Column<int>(nullable: false),
                    CategoryGroupId = table.Column<int>(nullable: false),
                    CategoryTypeId = table.Column<int>(nullable: false),
                    SpecialTagId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserServices_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserServices_CategoryGroups_CategoryGroupId",
                        column: x => x.CategoryGroupId,
                        principalTable: "CategoryGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServices_CategoryTypes_CategoryTypeId",
                        column: x => x.CategoryTypeId,
                        principalTable: "CategoryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServices_SpecialTags_SpecialTagId",
                        column: x => x.SpecialTagId,
                        principalTable: "SpecialTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_ApplicationUserId",
                table: "UserServices",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_CategoryGroupId",
                table: "UserServices",
                column: "CategoryGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_CategoryTypeId",
                table: "UserServices",
                column: "CategoryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_SpecialTagId",
                table: "UserServices",
                column: "SpecialTagId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServices");
        }
    }
}
