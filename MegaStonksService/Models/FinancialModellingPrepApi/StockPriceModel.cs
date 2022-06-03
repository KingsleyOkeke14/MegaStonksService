using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.FinancialModellingPrepApi
{
    public class StockPriceModel
    {

        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }

    }
}
