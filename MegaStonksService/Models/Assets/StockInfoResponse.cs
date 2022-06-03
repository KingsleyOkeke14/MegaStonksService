using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class StockInfoResponse
    {
        public int StockId { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal? ChangesPercentage { get; set; }
        public decimal? Change { get; set; }
        public decimal? DayLow { get; set; }
        public decimal? DayHigh { get; set; }
        public decimal? YearHigh { get; set; }
        public decimal? YearLow { get; set; }
        public long? MarketCap { get; set; }
        public decimal? PriceAvg50 { get; set; }
        public decimal? PriceAvg200 { get; set; }
        public int? Volume { get; set; }
        public int? AvgVolume { get; set; }
        public string Exchange { get; set; }
        public decimal? Open { get; set; }
        public decimal? PreviousClose { get; set; }
        public bool IsInWatchList { get; set; }
        public bool IsInPortfolio { get; set; }
    }
}
