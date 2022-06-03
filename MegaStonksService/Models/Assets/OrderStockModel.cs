using MegaStonksService.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class OrderStockModel
    {
        public int StockId { get; set; }
        public string OrderType { get; set; }
        public string OrderAction { get; set; }
        public int QuantitySubmitted { get; set; }
    }
}
