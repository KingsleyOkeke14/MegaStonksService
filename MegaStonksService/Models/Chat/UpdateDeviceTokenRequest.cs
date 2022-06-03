using MegaStonksService.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Chat
{
    public class UpdateDeviceTokenRequest
    {
        public ChatUser ChatUser { get; set; }
        public string DeviceToken { get; set; }
    }
}
