using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Entities.Assets
{
    public class Ad
    {
         public int Id { get; set; }
         public string Company { get; set; }
         public string Title { get; set; }
         public string Description { get; set; }
         public string ImageUrl { get; set; }
         public string UrlToLoad { get; set; }
         public DateTime? DateAdded { get; set; }
         public DateTime? ExpiryDate { get; set; }
         public DateTime? LastUpdated { get; set; }
    }
}