using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Assets
{
    public class CreateAdModel
    {
        public string Company { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UrlToLoad { get; set; }
        public int ExpiryIndays { get; set; }
    }
}