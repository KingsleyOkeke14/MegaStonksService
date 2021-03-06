// <auto-generated />
using System;
using MegaStonksService.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MegaStonksService.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210526224341_CryptoTables")]
    partial class CryptoTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Ad", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Company")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UrlToLoad")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Ads");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Crypto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<double?>("CirculatingSupply")
                        .HasColumnType("float");

                    b.Property<int?>("CmcRank")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<double?>("MaxSupply")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("TotalSupply")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Cryptos");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CryptoId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Reddit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Twitter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Website")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CryptoId");

                    b.ToTable("CryptoInfo");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoQuoteCAD", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CryptoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<double>("MarketCap")
                        .HasColumnType("float");

                    b.Property<decimal>("PercentChange1h")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange24h")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange30d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange60d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange7d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange90d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<double>("Volume24h")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CryptoId");

                    b.ToTable("CryptoQuoteCAD");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoQuoteUSD", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("CryptoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<double>("MarketCap")
                        .HasColumnType("float");

                    b.Property<decimal>("PercentChange1h")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange24h")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange30d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange60d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange7d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentChange90d")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("Price")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<double>("Volume24h")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("CryptoId");

                    b.ToTable("CryptoQuoteUSD");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.DailyPortfolioValue", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("MoneyReturn")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentReturn")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("Value")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("DailyPortfolioValues");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("Commission")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<DateTime?>("DateFilled")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateSubmitted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("OrderAction")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderStatus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrderType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("PricePerShare")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("QuantityFilled")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("QuantitySubmitted")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<int?>("StockId")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalCost")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("TotalPriceFilled")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("StockId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Stock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<decimal?>("Beta")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("CompanyName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Exchange")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExchangeShortName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Industry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActivelyTrading")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEtf")
                        .HasColumnType("bit");

                    b.Property<decimal?>("LastAnnualDividend")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<long?>("MarketCap")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Sector")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("Volume")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.StockHolding", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<decimal>("AverageCost")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("MarketValue")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("MoneyReturnToday")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("MoneyReturnTotal")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentOfPortfolio")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentReturnToday")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("PercentReturnTotal")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("Quantity")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<int?>("StockId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("StockId");

                    b.ToTable("StockHoldings");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("CurrentAmount")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<decimal>("InitialAmount")
                        .HasPrecision(18, 10)
                        .HasColumnType("decimal(18,10)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.WatchList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateAdded")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StockId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("StockId");

                    b.ToTable("WatchLists");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Authentication.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<bool>("AcceptTerms")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("AccountLockTimeout")
                        .HasColumnType("datetime2");

                    b.Property<bool>("AccountLocked")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOnboarded")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LoginAttempt")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PasswordReset")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Updated")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Verified")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoInfo", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Assets.Crypto", "Crypto")
                        .WithMany()
                        .HasForeignKey("CryptoId");

                    b.Navigation("Crypto");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoQuoteCAD", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Assets.Crypto", "Crypto")
                        .WithMany()
                        .HasForeignKey("CryptoId");

                    b.Navigation("Crypto");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.CryptoQuoteUSD", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Assets.Crypto", "Crypto")
                        .WithMany()
                        .HasForeignKey("CryptoId");

                    b.Navigation("Crypto");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.DailyPortfolioValue", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Authentication.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Order", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Authentication.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("MegaStonksService.Entities.Assets.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId");

                    b.Navigation("Account");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.StockHolding", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Authentication.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("MegaStonksService.Entities.Assets.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId");

                    b.Navigation("Account");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.Wallet", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Authentication.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.Navigation("Account");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Assets.WatchList", b =>
                {
                    b.HasOne("MegaStonksService.Entities.Authentication.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("MegaStonksService.Entities.Assets.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId");

                    b.Navigation("Account");

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("MegaStonksService.Entities.Authentication.Account", b =>
                {
                    b.OwnsMany("MegaStonksService.Entities.Authentication.RefreshToken", "RefreshTokens", b1 =>
                        {
                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .UseIdentityColumn();

                            b1.Property<int>("AccountId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("Created")
                                .HasColumnType("datetime2");

                            b1.Property<string>("CreatedByIp")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime>("Expires")
                                .HasColumnType("datetime2");

                            b1.Property<string>("ReplacedByToken")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime?>("Revoked")
                                .HasColumnType("datetime2");

                            b1.Property<string>("RevokedByIp")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Token")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("Id");

                            b1.HasIndex("AccountId");

                            b1.ToTable("RefreshToken");

                            b1.WithOwner("Account")
                                .HasForeignKey("AccountId");

                            b1.Navigation("Account");
                        });

                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
