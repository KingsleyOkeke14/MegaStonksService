using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.FinancialModellingPrepApi
{
    public class HistoricalPriceChartModel
    {
        public string Symbol { get; set; }
        public List<Historical> Historical { get; set; }

    }
    public class Historical
    {
        public string Date { get; set; }
        public double Close { get; set; }
    }
}
