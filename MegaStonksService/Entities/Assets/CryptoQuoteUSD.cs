using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class CryptoQuoteUSD
    {
        public int Id { get; set; }
        public Crypto Crypto { get; set; }
        public decimal Price { get; set; }

        public double Volume24h { get; set; }

        public decimal PercentChange1h { get; set; }

        public decimal PercentChange24h { get; set; }

        public decimal PercentChange7d { get; set; }

        public decimal PercentChange30d { get; set; }

        public decimal PercentChange60d { get; set; }

        public decimal PercentChange90d { get; set; }

        public double MarketCap { get; set; }

        public DateTime LastUpdated { get; set; }

    }
}
