using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MegaStonksService.Models.CoinMarketCapApi
{
    public class CryptoInfoModel
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("Symbol")]
        public SymbolInfo Symbol { get; set; }
    }
    public class SymbolInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("subreddit")]
        public string Subreddit { get; set; }

        [JsonProperty("notice")]
        public string Notice { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("tag-names")]
        public List<string> TagNames { get; set; }

        [JsonProperty("tag-groups")]
        public List<string> TagGroups { get; set; }

        [JsonProperty("urls")]
        public Urls Urls { get; set; }

        [JsonProperty("platform")]
        public object Platform { get; set; }

        [JsonProperty("date_added")]
        public DateTime DateAdded { get; set; }

        [JsonProperty("twitter_username")]
        public string TwitterUsername { get; set; }

        [JsonProperty("is_hidden")]
        public int IsHidden { get; set; }
    }

    public class Urls
    {
        [JsonProperty("website")]
        public List<string> Website { get; set; }

        [JsonProperty("twitter")]
        public List<object> Twitter { get; set; }

        [JsonProperty("message_board")]
        public List<string> MessageBoard { get; set; }

        [JsonProperty("chat")]
        public List<object> Chat { get; set; }

        [JsonProperty("explorer")]
        public List<string> Explorer { get; set; }

        [JsonProperty("reddit")]
        public List<string> Reddit { get; set; }

        [JsonProperty("technical_doc")]
        public List<string> TechnicalDoc { get; set; }

        [JsonProperty("source_code")]
        public List<string> SourceCode { get; set; }

        [JsonProperty("announcement")]
        public List<object> Announcement { get; set; }
    }

    public class CryptoInfoResolver : DefaultContractResolver
    {
        private string propertyName;

        public CryptoInfoResolver()
        {
        }
        public CryptoInfoResolver(string name)
        {
            propertyName = name;
        }
        public static readonly CryptoInfoResolver Instance = new CryptoInfoResolver();

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
