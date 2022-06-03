using MegaStonksService.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class StockComparer : IEqualityComparer<Stock>
    {
        public bool Equals(Stock x, Stock y)
        {
            return Equals(x.Symbol, y.Symbol);
        }

        public int GetHashCode(Stock obj)
        {
            return obj.Symbol.GetHashCode();
        }
    }
}
