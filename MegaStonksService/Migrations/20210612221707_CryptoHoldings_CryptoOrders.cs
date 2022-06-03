using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MegaStonksService.Migrations
{
    public partial class CryptoHoldings_CryptoOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoHoldings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CryptoId = table.Column<int>(type: "int", nullable: true),
                    AverageCost = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    MarketValue = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    PercentReturnToday = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    MoneyReturnToday = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    PercentReturnTotal = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    MoneyReturnTotal = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    PercentOfPortfolio = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoHoldings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoHoldings_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CryptoHoldings_Cryptos_CryptoId",
                        column: x => x.CryptoId,
                        principalTable: "Cryptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CryptoOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CryptoId = table.Column<int>(type: "int", nullable: true),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantitySubmitted = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    QuantityFilled = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    Commission = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    PricePerShare = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    TotalPriceFilled = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,10)", precision: 18, scale: 10, nullable: false),
                    DateSubmitted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateFilled = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CryptoOrders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CryptoOrders_Cryptos_CryptoId",
                        column: x => x.CryptoId,
                        principalTable: "Cryptos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldings_AccountId",
                table: "CryptoHoldings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoHoldings_CryptoId",
                table: "CryptoHoldings",
                column: "CryptoId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoOrders_AccountId",
                table: "CryptoOrders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CryptoOrders_CryptoId",
                table: "CryptoOrders",
                column: "CryptoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoHoldings");

            migrationBuilder.DropTable(
                name: "CryptoOrders");
        }
    }
}
