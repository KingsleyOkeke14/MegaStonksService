using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class OrderCryptoModel
    {
        public int CryptoId { get; set; }
        public string OrderType { get; set; }
        public string OrderAction { get; set; }
        public decimal QuantitySubmitted { get; set; }
    }
}
