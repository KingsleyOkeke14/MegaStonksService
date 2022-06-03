using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.FinancialModellingPrepApi
{
    public class StockNewsModel
    {
        public string Symbol { get; set; }
        public string PublishedDate { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Site { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }
}