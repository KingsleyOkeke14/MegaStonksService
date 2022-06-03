using MegaStonksService.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class Wallet
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public string Currency { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}