using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.FinancialModellingPrepApi
{
    public class StockModel : IEquatable<StockModel>
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
        public long? MarketCap { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
        public decimal? Beta { get; set; }
        public decimal? Price { get; set; }
        public decimal? LastAnnualDividend { get; set; }
        public long? Volume { get; set; }
        public string Exchange { get; set; }
        public string ExchangeShortName { get; set; }
        public string Country { get; set; }
        public bool IsEtf { get; set; }
        public bool IsActivelyTrading { get; set; }


        public bool Equals(StockModel other)
        {

            //Check whether the compared object is null.
            if (Object.ReferenceEquals(other, null)) return false;

            //Check whether the compared object references the same data.
            if (Object.ReferenceEquals(this, other)) return true;

            //Check whether the 'stocks' properties are equal.
            return Symbol.Equals(other.Symbol) && CompanyName.Equals(other.CompanyName)
                && ExchangeShortName.Equals(other.ExchangeShortName) && Country.Equals(other.Country);
        }

        public override int GetHashCode()
        {

            //Get hash code for the Symbol field if it is not null.
            int hashStockSymbol = Symbol == null ? 0 : Symbol.GetHashCode();

            int hashStockName = CompanyName == null ? 0 : Symbol.GetHashCode();
            int hashStockExchange = Exchange == null ? 0 : Symbol.GetHashCode();
            int hashStockCountry = Country == null ? 0 : Symbol.GetHashCode();

            //Calculate the hash code for the product.
            return hashStockSymbol ^ hashStockName
                   ^ hashStockExchange ^ hashStockCountry;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StockModel);
        }
    }
}