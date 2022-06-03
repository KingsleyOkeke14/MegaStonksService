using MegaStonksService.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Chat
{
    public class PostChatMessageRequest
    {
        public ChatUser Sender { get; set; }
        public int SessionId { get; set; }
        public string Message { get; set; }
    }
}