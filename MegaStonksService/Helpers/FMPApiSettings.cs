using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class FMPApiSettings
    {
        public string APIKey { get; set; }
        public string Stocks { get; set; }
        public string Forex { get; set; }
        public string Quote { get; set; }
        public string Price { get; set; }
        public string Profile { get; set; }
        public string Profile2 { get; set; }
        public string DailyPriceChart { get; set; }
        public string DailyPriceChartFull { get; set; }
        public string MarketHours { get; set; }
        public string StockNews { get; set; }
    }
}