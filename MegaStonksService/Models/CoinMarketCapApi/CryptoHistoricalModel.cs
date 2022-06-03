using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MegaStonksService.Models.CoinMarketCapApi
{
    public class CryptoHistoricalModel
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("data")]
        public HistoricalData Data { get; set; }

    }

        public class CurrencyQuote
        {
            [JsonProperty("open")]
            public double Open { get; set; }

            [JsonProperty("high")]
            public double High { get; set; }

            [JsonProperty("low")]
            public double Low { get; set; }

            [JsonProperty("close")]
            public double Close { get; set; }

            [JsonProperty("volume")]
            public object Volume { get; set; }

            [JsonProperty("market_cap")]
            public double? MarketCap { get; set; }

            [JsonProperty("timestamp")]
            public DateTime Timestamp { get; set; }
        }

        public class QuoteHistorical2
        {
            [JsonProperty("Currency")]
            public CurrencyQuote CurrencyQuote { get; set; }
        }

        public class QuoteHistorical
        {
            [JsonProperty("time_open")]
            public DateTime TimeOpen { get; set; }

            [JsonProperty("time_close")]
            public DateTime TimeClose { get; set; }

            [JsonProperty("time_high")]
            public DateTime TimeHigh { get; set; }

            [JsonProperty("time_low")]
            public DateTime TimeLow { get; set; }

            [JsonProperty("quote")]
            public QuoteHistorical2 Quote { get; set; }
        }

        public class HistoricalData
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("quotes")]
            public List<QuoteHistorical> Quotes { get; set; }
        }


    public class CryptoHistoricalResolver : DefaultContractResolver
    {
        private string propertyName;

        public CryptoHistoricalResolver()
        {
        }
        public CryptoHistoricalResolver(string name)
        {
            propertyName = name;
        }
        public static readonly CryptoHistoricalResolver Instance = new CryptoHistoricalResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "Currency")
            {
                property.PropertyName = propertyName;
            }
            return property;
        }
    }
}
