using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stock_api.Migrations
{
    public partial class amount_in_daily : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "DailyPrices");

            migrationBuilder.AddColumn<string>(
                name: "IsinCode",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubSector",
                table: "Stocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "DailyPrices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PurchasePrice",
                table: "DailyPrices",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsinCode",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "SubSector",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DailyPrices");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "DailyPrices");

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "Stocks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "DailyPrices",
                type: "datetime2",
                nullable: true);
        }
    }
}
