using System;
namespace MegaStonksService.Entities.Chat
{
    public class ChatUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string ConnectionId { get; set; }
        public bool IsConsultant { get; set; }
        public string DeviceToken { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}