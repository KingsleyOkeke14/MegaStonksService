using MegaStonksService.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Chat
{
    public class StartChatRequest
    {
        public ChatUser User { get; set; }
        public ChatUser UserToStartChatWith { get; set; }
    }
}
