using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class addAppointmentPersonToAppointmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppPersonId",
                table: "Appointments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppPersonId",
                table: "Appointments",
                column: "AppPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_AspNetUsers_AppPersonId",
                table: "Appointments",
                column: "AppPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_AspNetUsers_AppPersonId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_AppPersonId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "AppPersonId",
                table: "Appointments");
        }
    }
}
