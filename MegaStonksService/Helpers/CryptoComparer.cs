using MegaStonksService.Entities.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class CryptoComparer : IEqualityComparer<Crypto>
    {
            public bool Equals(Crypto x, Crypto y)
            {
                return Equals(x.Symbol, y.Symbol);
            }

            public int GetHashCode(Crypto obj)
            {
                return obj.Symbol.GetHashCode();
            }
    }
}
