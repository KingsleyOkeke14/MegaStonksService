using MegaStonksService.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class WatchList
    {
        public int Id { get; set; }
        public Account Account { get; set; }
        public Stock Stock { get; set; }

        public DateTime? DateAdded { get; set; }
    }
}