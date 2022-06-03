using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MegaStonksService.Entities.Chat;
using MegaStonksService.Helpers;
using MegaStonksService.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace MegaStonksService.Services
{
    public interface IChatService
    {
        ChatUser GetAdmin(string authcode);
        ChatUser CreateUser(string userName, string userImage);
        ChatUser GetChatUser(ChatUser user);
        ChatSession GetChatSession(int sessionId);
        void UpdateConnectionId(string userName, string connectionID);
        List<ChatFeed> GetChatFeed(int userId);
        ChatSession StartChatWithUser(ChatUser user, ChatUser userToChatWith);
        ChatMessageResponse SaveMessage(ChatUser sender, int sessionId, string message);
        string UpdateDeviceToken(ChatUser user, string deviceToken);
        string RemoveDeviceToken(ChatUser user);
    }
    public class ChatService : IChatService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        
        public ChatService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public ChatUser GetAdmin(string authcode)
        {
            try
            {
                if (authcode == "AABBGG1")
                {
                   return  _context.ChatUsers.Find(1);
                }
                else
                {
                    throw new AppException($"Could not Retrieve Admin: Invalid Code");
                }
            }
            catch (Exception e)
            {
                throw new AppException($"Could not Retrieve Admin: {e.Message}");
            }
        }

        public ChatUser CreateUser(string userName, string userImage)
        {
            try
            {
                if(userName == string.Empty || userImage == string.Empty
                    || userName == null || userImage == null)
                {
                    throw new AppException("Invalid Username or UserImage");
                }
                var existingUserName = _context.ChatUsers.AsNoTracking()
                                                        .Where(x => x.UserName.ToUpper().Trim() == userName.ToUpper().Trim())
                                                        .FirstOrDefault();

                
                if(existingUserName != null)
                {
                    throw new AppException("Username already Exists in Database");
                }
                var userToAdd = new ChatUser
                {
                    UserName = userName.Trim(),
                    Image = userImage,
                    IsConsultant = false,
                    LastUpdated = DateTime.UtcNow
                };

                _context.ChatUsers.Add(userToAdd);
                _context.SaveChanges();

                return _context.ChatUsers.AsNoTracking().Where(x => x.UserName == userName).First();

            }
            catch(Exception e)
            {
                throw new AppException($"Could not Create User: {e.Message}");
            }
        }

        public ChatSession StartChatWithUser(ChatUser user, ChatUser userToChatWith)
        {
            try
            {
                if(user.Id == userToChatWith.Id)
                {
                    throw new AppException("You cannot start a chat with yourself");
                }
                var userInDB = _context.ChatUsers.Where(x => x.Id == user.Id).First();
                var userToStartChatWithInDb = _context.ChatUsers.Where(x => x.Id == userToChatWith.Id).First();
                var existingChatSession = _context.ChatSessions.AsNoTracking().Where(x => x.User1 == userInDB && x.User2 == userToStartChatWithInDb
                                                                                       || x.User1 == userToStartChatWithInDb && x.User2 == userInDB).FirstOrDefault();
                if(existingChatSession != null)
                {
                    throw new AppException("Chat Session Already Exists");
                }
               var newChatSession = new ChatSession
                {
                    User1 = userInDB,
                    User2 = userToStartChatWithInDb,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ChatSessions.Add(newChatSession);
                _context.SaveChanges();
                var savedChatSession = _context.ChatSessions.AsNoTracking().Where(x => x.User1 == userInDB && x.User2 == userToStartChatWithInDb).First();
                return savedChatSession;
            }
            catch(Exception e)
            {
                throw new AppException($"Could Not Create Chat Session {e.Message}");
            }

        }

        public ChatUser GetChatUser(ChatUser user)
        {
            try
            {
                var userRetrieved = _context.ChatUsers.AsNoTracking().Where(x => x.Id == user.Id).First();
                return user;
            }
            catch
            {
                throw new AppException("Could not find User in DB");
            }
        }

        public ChatSession GetChatSession(int sessionId)
        {
            try
            {
                var sessionRetrieved = _context.ChatSessions.AsNoTracking().Include(x => x.User1).Include(x => x.User2).Where(x => x.Id == sessionId).First();
                return sessionRetrieved;
            }
            catch
            {
                throw new AppException("Could not Chat Session in DB");
            }
        }

        public void UpdateConnectionId(string userName, string connectionID)
        {
            try
            {
                var user = _context.ChatUsers.Where(x => x.UserName == userName).First();
                if(user.ConnectionId != connectionID)
                {
                    user.ConnectionId = connectionID;
                    _context.Update(user);
                    _context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new AppException($"Could not update User Connection ID {e.InnerException.Message}");
            }
        }

        public List<ChatFeed> GetChatFeed(int userId)
        {
            try
            {
                var userInDb = _context.ChatUsers.Where(x => x.Id == userId).First();
                var feeds = new List<ChatFeed>();
                if (!userInDb.IsConsultant)
                {
                    var consultants = _context.ChatUsers.Where(x => x.IsConsultant).ToList();
                    var chatSessions = _context.ChatSessions.Where(x => x.User1.Id == userInDb.Id).ToList();
                    foreach (var consultant in consultants)
                    {
                        var chatFeed = new ChatFeed
                        {
                            User = consultant,
                            Messages = new List<ChatMessageResponse>()
                        };
                        foreach (var session in chatSessions)
                        {
                            chatFeed.ChatSession = session;
                            var messagesInDb = _context.ChatMessages.Include(x => x.ChatUser).Where(x => x.ChatSession == session).ToList();
                            for (int index = 0; index < messagesInDb.Count; index++)
                            {
                                chatFeed.Messages.Add(new ChatMessageResponse
                                {
                                    ChatSessionId = session.Id,
                                    Message = messagesInDb[index].Message,
                                    IsReply = messagesInDb[index].ChatUser.Id == userInDb.Id ? false : true,
                                    IsRead = messagesInDb[index].IsRead,
                                    TimeStamp = messagesInDb[index].TimeStamp
                                });
                                if (index == messagesInDb.Count - 1)
                                {
                                    chatFeed.LastUpdated = messagesInDb[index].TimeStamp;
                                }
                            }
                        }
                        feeds.Add(chatFeed);
                    }
                }
                else
                {
                    var users = _context.ChatUsers.Where(x => !x.IsConsultant).ToList();
                    foreach (var user in users)
                    {
                        var chatSession = _context.ChatSessions.Where(x => x.User2 == userInDb && x.User1 == user).FirstOrDefault();

                        if(chatSession != null)
                        {
                            var chatFeed = new ChatFeed
                            {
                                User = user,
                                ChatSession = chatSession,
                                Messages = new List<ChatMessageResponse>()
                            };
                            var messagesInDb = _context.ChatMessages.Include(x => x.ChatUser).Where(x => x.ChatSession == chatSession).ToList();
                            for(int index = 0; index < messagesInDb.Count; index++)
                            {
                                chatFeed.Messages.Add(new ChatMessageResponse
                                {
                                    ChatSessionId = chatSession.Id,
                                    Message = messagesInDb[index].Message,
                                    IsReply = messagesInDb[index].ChatUser.Id == userInDb.Id ? false : true,
                                    IsRead = messagesInDb[index].IsRead,
                                    TimeStamp = messagesInDb[index].TimeStamp
                                });
                                if(index == messagesInDb.Count - 1)
                                {
                                    chatFeed.LastUpdated = messagesInDb[index].TimeStamp;
                                }
                            }
                            feeds.Add(chatFeed);
                        }
                    }
                }
                return feeds.OrderByDescending(x => x.LastUpdated).ToList();
            }
            catch(Exception e)
            {
                throw new AppException($"Could not retrieve user feed at this time {e.InnerException.Message}");
            }
        }

        public ChatMessageResponse SaveMessage(ChatUser sender, int sessionId, string message)
        {
            try
            {
                var senderInDb = _context.ChatUsers.Where(x => x.Id == sender.Id).First();
                var chatSessionInDb = _context.ChatSessions.Find(sessionId);
                var newMessage = new ChatMessage
                {
                    ChatUser = senderInDb,
                    ChatSession = chatSessionInDb,
                    Message = message,
                    IsRead = false,
                    TimeStamp = DateTime.UtcNow
                };

                _context.ChatMessages.Add(newMessage);
                _context.SaveChanges();

                var response = new ChatMessageResponse
                {
                    ChatSessionId = chatSessionInDb.Id,
                    Message = newMessage.Message,
                    IsReply = false,
                    IsRead = newMessage.IsRead,
                    TimeStamp = newMessage.TimeStamp
                };
                return response;
            }
            catch(Exception e)
            {
                throw new AppException($"Could not save chat {e.Message}");
            }
        }

        public string UpdateDeviceToken(ChatUser user, string deviceToken)
        {
            try
            {
                var userRetrieved = _context.ChatUsers.AsNoTracking().Where(x => x.Id == user.Id).First();


                if (deviceToken != userRetrieved.DeviceToken)
                {
                    userRetrieved.DeviceToken = deviceToken;
                    _context.Update(userRetrieved);
                    _context.SaveChanges();
                    return deviceToken;
                }
                else
                {
                    return deviceToken;
                }
            }
            catch(Exception e)
            {
                throw new AppException($"Could not update device token {e.Message}");
            }
        }

        public string RemoveDeviceToken(ChatUser user)
        {
            try
            {
                var userRetrieved = _context.ChatUsers.AsNoTracking().Where(x => x.Id == user.Id).First();

                    userRetrieved.DeviceToken = "";
                    _context.Update(userRetrieved);
                    _context.SaveChanges();
                    return "";
                
            }
            catch (Exception e)
            {
                throw new AppException($"Could not update device token {e.Message}");
            }
        }
    }
}