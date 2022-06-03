using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class Stock
    {
        public int Id { get; set; }
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public long? MarketCap { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public decimal? Beta { get; set; }
        public decimal? Price { get; set; }
        public decimal? LastAnnualDividend { get; set; }
        public long? Volume { get; set; }
        public string Exchange { get; set; }
        public string ExchangeShortName { get; set; }
        public string Country { get; set; }
        public bool IsEtf { get; set; }
        public bool IsActivelyTrading { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
