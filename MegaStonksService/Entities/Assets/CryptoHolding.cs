using MegaStonksService.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class CryptoHolding
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public Crypto Crypto { get; set; }
        public decimal AverageCost { get; set; }
        public decimal Quantity { get; set; }
        public decimal MarketValue { get; set; }
        public decimal PercentReturnToday { get; set; }
        public decimal MoneyReturnToday { get; set; }
        public decimal PercentReturnTotal { get; set; }
        public decimal MoneyReturnTotal { get; set; }
        public decimal PercentOfPortfolio { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}