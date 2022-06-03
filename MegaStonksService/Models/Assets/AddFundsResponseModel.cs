using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class AddFundsResponseModel
    {
        public decimal PreviousBalance { get; set; }
        public decimal AmountAdded { get; set; }
        public decimal NewBalance { get; set; } 
    }
}
