using Microsoft.EntityFrameworkCore.Migrations;

namespace Colibri.Migrations
{
    public partial class changeRatingToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ServiceRating",
                table: "UserServices",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfServiceRates",
                table: "UserServices",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "ProductRating",
                table: "Products",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfProductRates",
                table: "Products",
                nullable: true,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfServiceRates",
                table: "UserServices");

            migrationBuilder.DropColumn(
                name: "NumberOfProductRates",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceRating",
                table: "UserServices",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<string>(
                name: "ProductRating",
                table: "Products",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
