using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MegaStonksService.Models.CoinMarketCapApi
{
    public class CryptoQuoteModel
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        public SymbolQuote Data { get; set; }
    }

    public class SymbolQuote
    {
        [JsonProperty("Symbol")]
        public CryptoModel Symbol { get; set; }
    }

    public class CryptoQuoteResolver : DefaultContractResolver
    {
        private string propertyName;

        public CryptoQuoteResolver()
        {
        }
        public CryptoQuoteResolver(string name)
        {
            propertyName = name;
        }
        public static readonly CryptoQuoteResolver Instance = new CryptoQuoteResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "Symbol")
            {
                property.PropertyName = propertyName;
            }
            return property;
        }
    }
}