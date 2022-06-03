using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class ForexModel
    {
        public string Ticker { get; set; }
        public string Bid { get; set; }
        public string Ask { get; set; }
        public string Open { get; set; }
        public string Low { get; set; }
        public string High { get; set; }
        public decimal Changes { get; set; }
        public string Date { get; set; }
    }
}
