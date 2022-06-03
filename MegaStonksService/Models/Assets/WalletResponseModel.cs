using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class WalletResponseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Cash { get; set; }
        public decimal InitialDeposit { get; set; }
        public decimal Investments { get; set; }
        public decimal Total { get; set; }
        public decimal PercentReturnToday { get; set; }
        public decimal MoneyReturnToday { get; set; }
        public decimal PercentReturnTotal { get; set; }
        public decimal MoneyReturnTotal { get; set; }
    }
}
