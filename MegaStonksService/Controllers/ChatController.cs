using System;
using System.Collections.Generic;
using MegaStonksService.Entities.Chat;
using MegaStonksService.Models.Chat;
using MegaStonksService.Services;
using Microsoft.AspNetCore.Mvc;

namespace MegaStonksService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("getAdmin")]
        public ActionResult<ChatUser> GetAdmin(string authCode)
        {
            var result = _chatService.GetAdmin(authCode);
            return Ok(result);
        }

        [HttpPut("createUser")]
        public ActionResult<ChatUser> CreateUser(string userName, string userImage)
        {
            var result = _chatService.CreateUser(userName, userImage);
            return Ok(result);
        }

        [HttpGet("getFeed")]
        public ActionResult<IEnumerable<ChatFeed>> GetFeed(int userId)
        {
            var result = _chatService.GetChatFeed(userId);
            return Ok(result);
        }

        [HttpPost("startChat")]
        public ActionResult<ChatSession> StartChat(StartChatRequest startChatRequest)
        {
            var result = _chatService.StartChatWithUser(startChatRequest.User, startChatRequest.UserToStartChatWith);
            return Ok(result);
        }

        [HttpPost("saveMessage")]
        public ActionResult<string> SaveMessage(PostChatMessageRequest postMessageRequest)
        {
            var result = _chatService.SaveMessage(postMessageRequest.Sender, postMessageRequest.SessionId, postMessageRequest.Message);
            return Ok(result);
        }

        [HttpPost("updateDeviceToken")]
        public ActionResult<string> UpdateDeviceToken(UpdateDeviceTokenRequest updateDeviceTokenRequest)
        {
            var result = _chatService.UpdateDeviceToken(updateDeviceTokenRequest.ChatUser, updateDeviceTokenRequest.DeviceToken);
            return Ok(result);
        }

        [HttpPost("removeDeviceToken")]
        public ActionResult<string> RemoveDeviceToken(ChatUser user)
        {
            var result = _chatService.RemoveDeviceToken(user);
            return Ok(result);
        }
    }
}