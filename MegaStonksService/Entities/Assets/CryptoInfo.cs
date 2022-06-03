using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class CryptoInfo
    {
        public int Id { get; set; }
        public Crypto Crypto { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Website { get; set; }
        public string Twitter { get; set; }
        public string Reddit { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}