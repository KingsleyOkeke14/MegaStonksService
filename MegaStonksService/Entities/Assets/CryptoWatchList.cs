using MegaStonksService.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class CryptoWatchList
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public Crypto Crypto { get; set; }

        public DateTime? DateAdded { get; set; }
    }
}
