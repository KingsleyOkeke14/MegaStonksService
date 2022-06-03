using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class PriceChartResponse
    {
        public string Date { get; set; }
        public double Close { get; set; }
    }
}
