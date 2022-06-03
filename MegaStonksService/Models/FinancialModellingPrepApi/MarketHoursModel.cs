using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.FinancialModellingPrepApi
{

    public class MarketHoursModel
    {
        public string StockExchangeName { get; set; }
        public StockMarketHours StockMarketHours { get; set; }
        public List<StockMarketHoliday> StockMarketHolidays { get; set; }
        public bool IsTheStockMarketOpen { get; set; }
        public bool IsTheEuronextMarketOpen { get; set; }
        public bool IsTheForexMarketOpen { get; set; }
        public bool IsTheCryptoMarketOpen { get; set; }
    }

    public class StockMarketHours
    {
        public string OpeningHour { get; set; }
        public string ClosingHour { get; set; }
    }

    public class StockMarketHoliday
    {
        public int Year { get; set; }
        public string NewYearsDay { get; set; }

        [JsonProperty("Martin Luther King, Jr. Day")]
        public string MartinLutherKingJrDay { get; set; }

        [JsonProperty("Washington's Birthday")]
        public string WashingtonSBirthday { get; set; }
        public string GoodFriday { get; set; }
        public string MemorialDay { get; set; }
        public string IndependenceDay { get; set; }
        public string LaborDay { get; set; }
        public string ThanksgivingDay { get; set; }
        public string Christmas { get; set; }
    }
}