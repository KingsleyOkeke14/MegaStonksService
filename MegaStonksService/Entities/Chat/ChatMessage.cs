using System;
namespace MegaStonksService.Entities.Chat
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public ChatUser ChatUser { get; set; }
        public ChatSession ChatSession { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}