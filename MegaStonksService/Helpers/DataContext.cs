using MegaStonksService.Entities.Assets;
using MegaStonksService.Entities.Authentication;
using MegaStonksService.Entities.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<WatchList> WatchLists { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<StockHolding> StockHoldings { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<DailyPortfolioValue> DailyPortfolioValues { get; set; }
        public DbSet<Crypto> Cryptos { get; set; }
        public DbSet<CryptoInfo> CryptoInfo { get; set; }
        public DbSet<CryptoQuoteUSD> CryptoQuoteUSD { get; set; }
        public DbSet<CryptoQuoteCAD> CryptoQuoteCAD { get; set; }
        public DbSet<CryptoWatchList> CryptoWatchLists { get; set; }
        public DbSet<CryptoHolding> CryptoHoldings { get; set; }
        public DbSet<CryptoOrder> CryptoOrders { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to SqlServer database
            options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal)))
            {
                property.SetPrecision(18);
                property.SetScale(10);
            }

        }
    }
}