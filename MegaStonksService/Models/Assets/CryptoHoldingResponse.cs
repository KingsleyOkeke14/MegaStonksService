using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class CryptoHoldingResponse
    {
        public int Id { get; set; }
        public int CryptoId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Logo { get; set; }
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
