using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Colibri.Migrations
{
    public partial class TransportCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Transports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CareOf = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    HouseNumber = table.Column<int>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    Zip = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    TransportId1 = table.Column<int>(nullable: true),
                    UserIdId = table.Column<int>(nullable: true),
                    Caravan_TransportId1 = table.Column<int>(nullable: true),
                    Miscellaneous_TransportId1 = table.Column<int>(nullable: true),
                    Motorcycle_TransportId1 = table.Column<int>(nullable: true),
                    Oldtimer_TransportId1 = table.Column<int>(nullable: true),
                    Parking_TransportId1 = table.Column<int>(nullable: true),
                    Tractor_TransportId1 = table.Column<int>(nullable: true),
                    Trailer_TransportId1 = table.Column<int>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: true),
                    CargoVolume = table.Column<double>(nullable: false),
                    Color = table.Column<string>(nullable: true),
                    ConstructionYear = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    FuelCapacity = table.Column<double>(nullable: false),
                    FuelConsumption = table.Column<double>(nullable: false),
                    FuelType = table.Column<string>(nullable: true),
                    Model = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    TransportId = table.Column<int>(nullable: true),
                    Vespa_TransportId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_TransportId1",
                        column: x => x.TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_User_UserIdId",
                        column: x => x.UserIdId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Caravan_TransportId1",
                        column: x => x.Caravan_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Miscellaneous_TransportId1",
                        column: x => x.Miscellaneous_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Motorcycle_TransportId1",
                        column: x => x.Motorcycle_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Oldtimer_TransportId1",
                        column: x => x.Oldtimer_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Parking_TransportId1",
                        column: x => x.Parking_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Tractor_TransportId1",
                        column: x => x.Tractor_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Trailer_TransportId1",
                        column: x => x.Trailer_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_TransportId",
                        column: x => x.TransportId,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vehicle_Transports_Vespa_TransportId1",
                        column: x => x.Vespa_TransportId1,
                        principalTable: "Transports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_TransportId1",
                table: "Vehicle",
                column: "TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_UserIdId",
                table: "Vehicle",
                column: "UserIdId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Caravan_TransportId1",
                table: "Vehicle",
                column: "Caravan_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Miscellaneous_TransportId1",
                table: "Vehicle",
                column: "Miscellaneous_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Motorcycle_TransportId1",
                table: "Vehicle",
                column: "Motorcycle_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Oldtimer_TransportId1",
                table: "Vehicle",
                column: "Oldtimer_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Parking_TransportId1",
                table: "Vehicle",
                column: "Parking_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Tractor_TransportId1",
                table: "Vehicle",
                column: "Tractor_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Trailer_TransportId1",
                table: "Vehicle",
                column: "Trailer_TransportId1");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_TransportId",
                table: "Vehicle",
                column: "TransportId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Vespa_TransportId1",
                table: "Vehicle",
                column: "Vespa_TransportId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Transports");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
