using System;
using System.Threading.Tasks;
using MegaStonksService.Entities.Chat;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Chat;
using MegaStonksService.Services;
using Microsoft.AspNetCore.SignalR;

namespace MegaStonksService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IApplePushNotificationService _applePushNotificationService;

        public ChatHub(IChatService chatService, IApplePushNotificationService applePushNotificationService)
        {
            _chatService = chatService;
            _applePushNotificationService = applePushNotificationService;

        }
        public async Task<ChatMessageResponse>  SendMessage(PostChatMessageRequest messageRequest)
        {
            try
            { 
                if(messageRequest.Sender.ConnectionId != Context.ConnectionId)
                {
                    UpdateConnectionID(messageRequest.Sender.UserName);
                }
                var session = _chatService.GetChatSession(messageRequest.SessionId);
                var receipient = messageRequest.Sender.Id == session.User1.Id ? session.User2 : session.User1;
                var chatUser = _chatService.GetChatUser(receipient);

                var response = _chatService.SaveMessage(messageRequest.Sender, messageRequest.SessionId, messageRequest.Message);
                if(chatUser.ConnectionId != null)
                {
                    await Clients.Client(chatUser.ConnectionId).SendAsync("ReceiveMessage", response);
                    _applePushNotificationService.SendPushForNewChatMessage(chatUser, messageRequest.Sender);
                }
               
                return response;
            }
            catch(Exception e)
            {
                throw new AppException($"Could not send Message to User {e.InnerException.Message}");
            }
        }

        public void UpdateConnectionID(string user)
        {
            _chatService.UpdateConnectionId(user, Context.ConnectionId);
        }
    }
}