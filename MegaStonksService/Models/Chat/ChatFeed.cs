using MegaStonksService.Entities.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MegaStonksService.Models.Chat
{
    public class ChatFeed
    {
        public ChatUser User { get; set; }
        public ChatSession ChatSession { get; set; }
        public List<ChatMessageResponse> Messages { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ChatMessageResponse
    {
        public int ChatSessionId { get; set; }
        public string Message { get; set; }
        public bool IsReply { get; set; }
        public bool IsRead { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}