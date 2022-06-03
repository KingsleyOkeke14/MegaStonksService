using System;
using Newtonsoft.Json;

namespace MegaStonksService.Models.Utils
{
 
     public class AppleNotification
     {
         public class ApsPayload
         {
             [JsonProperty("alert")]
             public string AlertBody { get; set; }

             [JsonProperty("sound")]
             public string Sound { get; set; }
        }

            // Your custom properties as needed

            [JsonProperty("aps")]
            public ApsPayload Aps { get; set; }
      }

}