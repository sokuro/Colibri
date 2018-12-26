using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class changeServicesForAppointmentToUserServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicesSelectedForAppointment");

            migrationBuilder.CreateTable(
                name: "UserServicesSelectedForAppointment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppointmentId = table.Column<int>(nullable: false),
                    UserServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServicesSelectedForAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserServicesSelectedForAppointment_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServicesSelectedForAppointment_UserServices_UserServiceId",
                        column: x => x.UserServiceId,
                        principalTable: "UserServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserServicesSelectedForAppointment_AppointmentId",
                table: "UserServicesSelectedForAppointment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServicesSelectedForAppointment_UserServiceId",
                table: "UserServicesSelectedForAppointment",
                column: "UserServiceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserServicesSelectedForAppointment");

            migrationBuilder.CreateTable(
                name: "ServicesSelectedForAppointment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppointmentId = table.Column<int>(nullable: false),
                    UserServiceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesSelectedForAppointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicesSelectedForAppointment_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicesSelectedForAppointment_UserServices_UserServiceId",
                        column: x => x.UserServiceId,
                        principalTable: "UserServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicesSelectedForAppointment_AppointmentId",
                table: "ServicesSelectedForAppointment",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesSelectedForAppointment_UserServiceId",
                table: "ServicesSelectedForAppointment",
                column: "UserServiceId");
        }
    }
}
