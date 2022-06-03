using System;

namespace MegaStonksService.Entities.Chat
{
    public class ChatSession
    {
        public int Id { get; set; }
        public ChatUser User1 { get; set; }
        public ChatUser User2 { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}