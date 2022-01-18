using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace stock_api.Migrations
{
    public partial class change_in_dailyprice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPrice_Stocks_StockTicker",
                table: "DailyPrice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPrice",
                table: "DailyPrice");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Stocks");

            migrationBuilder.RenameTable(
                name: "DailyPrice",
                newName: "DailyPrices");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPrice_StockTicker",
                table: "DailyPrices",
                newName: "IX_DailyPrices_StockTicker");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPrices",
                table: "DailyPrices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPrices_Stocks_StockTicker",
                table: "DailyPrices",
                column: "StockTicker",
                principalTable: "Stocks",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPrices_Stocks_StockTicker",
                table: "DailyPrices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyPrices",
                table: "DailyPrices");

            migrationBuilder.RenameTable(
                name: "DailyPrices",
                newName: "DailyPrice");

            migrationBuilder.RenameIndex(
                name: "IX_DailyPrices_StockTicker",
                table: "DailyPrice",
                newName: "IX_DailyPrice_StockTicker");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "Stocks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyPrice",
                table: "DailyPrice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPrice_Stocks_StockTicker",
                table: "DailyPrice",
                column: "StockTicker",
                principalTable: "Stocks",
                principalColumn: "Ticker",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
