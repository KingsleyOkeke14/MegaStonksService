using MegaStonksService.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class DailyPortfolioValue
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public decimal Value { get; set; }
        public decimal PercentReturn { get; set; }
        public decimal MoneyReturn { get; set; }
        public DateTime? Date { get; set; }
    }
}
