using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class StockHoldingResponse
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public decimal? ChangesPercentage { get; set; }
        public decimal AverageCost { get; set; }
        public decimal Quantity { get; set; }
        public decimal MarketValue { get; set; }
        public decimal PercentReturnToday { get; set; }
        public decimal MoneyReturnToday { get; set; }
        public decimal PercentReturnTotal { get; set; }
        public decimal MoneyReturnTotal { get; set; }
        public decimal PercentOfPortfolio { get; set; }
    }
}
