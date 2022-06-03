using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class OrdersResponse
    {

        public int Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderAction { get; set; }
        public decimal QuantitySubmitted { get; set; }
        public decimal QuantityFilled { get; set; }
        public decimal Commission { get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalPriceFilled { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DateFilled { get; set; }
    }
}
