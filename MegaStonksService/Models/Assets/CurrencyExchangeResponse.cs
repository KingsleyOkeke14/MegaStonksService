using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class CurrencyExchangeResponse
    {
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public string CurrencyPair { get; set; }
        public decimal Result { get; set; }
    }
}
