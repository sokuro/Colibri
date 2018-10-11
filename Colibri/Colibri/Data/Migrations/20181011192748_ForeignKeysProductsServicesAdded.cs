using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Colibri.Migrations
{
    public partial class ForeignKeysProductsServicesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Categories",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceId",
                table: "Categories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProductId",
                table: "Categories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ServiceId",
                table: "Categories",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Products_ProductId",
                table: "Categories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Services_ServiceId",
                table: "Categories",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Products_ProductId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Services_ServiceId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ProductId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ServiceId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                table: "Categories");
        }
    }
}
