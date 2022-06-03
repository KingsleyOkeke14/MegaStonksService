using MegaStonksService.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class OrderStockResponseModel
    {
        public string Name { get; set; }
        public string StockSymbol { get; set; }
        public string Currency { get; set; }
        public string OrderType { get; set; }
        public string OrderStatus { get; set; }
        public string OrderAction { get; set; }
        public decimal QuantitySubmitted { get; set; }
        public decimal QuantityFilled { get; set; }
        public decimal Commission { get; set; }
        public decimal PricePerShare { get; set; }
        public decimal TotalPriceFilled { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ForexExchangeRate { get; set; }
        public decimal ExchangeResult { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime DateFilled { get; set; }
    }
}