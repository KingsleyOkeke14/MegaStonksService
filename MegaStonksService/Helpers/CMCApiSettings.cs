using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Helpers
{
    public class CMCApiSettings
    {
        public string APIKey { get; set; }
        public string Latest { get; set; }
        public string Info { get; set; }
        public string Quote { get; set; }
        public string Historical { get; set; }
    }
}
