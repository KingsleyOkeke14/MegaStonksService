using MegaStonksService.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{

    public class CryptoResponseModel
    {
        public Crypto Crypto { get; set; }
        public CryptoInfoResponse Info { get; set; }
        public CryptoQuoteResponse USDQuote { get; set; }
        public CryptoQuoteResponse CADQuote { get; set; }
        public bool IsInWatchlist { get; set; }
        public bool IsInPortfolio { get; set; }
    }
    public class CryptoInfoResponse
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Reddit { get; set; }
    }

    public class CryptoQuoteResponse
    {
        public decimal Price { get; set; }

        public double Volume24h { get; set; }

        public decimal PercentChange1h { get; set; }

        public decimal PercentChange24h { get; set; }

        public decimal PercentChange7d { get; set; }

        public decimal PercentChange30d { get; set; }

        public decimal PercentChange60d { get; set; }

        public decimal PercentChange90d { get; set; }

        public double MarketCap { get; set; }
    }
}