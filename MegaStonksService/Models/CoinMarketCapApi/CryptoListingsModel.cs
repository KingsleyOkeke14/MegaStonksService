using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.CoinMarketCapApi
{
    public class CryptoListingsModel
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("data")]
        public List<CryptoModel> Data { get; set; }
    }

    public class Status
    {
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_message")]
        public object ErrorMessage { get; set; }

        [JsonProperty("elapsed")]
        public double Elapsed { get; set; }

        [JsonProperty("credit_count")]
        public int CreditCount { get; set; }

        [JsonProperty("notice")]
        public object Notice { get; set; }

        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
    }

    public class Platform
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("token_address")]
        public string TokenAddress { get; set; }
    }

    public class QuoteModel
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("volume_24h")]
        public double? Volume24h { get; set; }

        [JsonProperty("percent_change_1h")]
        public decimal? PercentChange1h { get; set; }

        [JsonProperty("percent_change_24h")]
        public decimal? PercentChange24h { get; set; }

        [JsonProperty("percent_change_7d")]
        public decimal? PercentChange7d { get; set; }

        [JsonProperty("percent_change_30d")]
        public decimal? PercentChange30d { get; set; }

        [JsonProperty("percent_change_60d")]
        public decimal? PercentChange60d { get; set; }

        [JsonProperty("percent_change_90d")]
        public decimal? PercentChange90d { get; set; }

        [JsonProperty("market_cap")]
        public double? MarketCap { get; set; }

        [JsonProperty("last_updated")]
        public DateTime LastUpdatedTime { get; set; }
    }

    public class Quote
    {
        [JsonProperty("CAD")]
        public QuoteModel CAD { get; set; }

        [JsonProperty("USD")]
        public QuoteModel USD { get; set; }
    }



    public class CryptoModel : IEquatable<CryptoModel>
    {
        [JsonProperty("id")]
        public int CryptoId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("num_market_pairs")]
        public int NumMarketPairs { get; set; }

        [JsonProperty("date_added")]
        public DateTime DateAdded { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("max_supply")]
        public double? MaxSupply { get; set; }

        [JsonProperty("circulating_supply")]
        public double? CirculatingSupply { get; set; }

        [JsonProperty("total_supply")]
        public double? TotalSupply { get; set; }

        [JsonProperty("platform")]
        public Platform Platform { get; set; }

        [JsonProperty("cmc_rank")]
        public int CmcRank { get; set; }

        [JsonProperty("last_updated")]
        public DateTime LastUpdatedTime { get; set; }

        [JsonProperty("quote")]
        public Quote Quote { get; set; }

        public bool Equals(CryptoModel other)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the 'stocks' properties are equal.
            return Symbol.Equals(other.Symbol) && Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {

            //Get hash code for the Symbol field if it is not null.
            int hashCryptoSymbol = Symbol == null ? 0 : Symbol.GetHashCode();

            int hashCryptoName = Name == null ? 0 : Name.GetHashCode();

            //Calculate the hash code for the product.
            return hashCryptoSymbol ^ hashCryptoName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CryptoModel);
        }
    }
}