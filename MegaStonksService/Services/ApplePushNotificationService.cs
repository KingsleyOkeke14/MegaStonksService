using System;
using System.Net.Http;
using MegaStonksService.Helpers;
using Microsoft.Extensions.Options;
using CorePush.Apple;
using MegaStonksService.Entities.Chat;
using MegaStonksService.Models.Utils;

namespace MegaStonksService.Services
{
    public interface IApplePushNotificationService
    {
        void SendPushForNewChatMessage(ChatUser chatUser, ChatUser sender);
    }
    public class ApplePushNotificationService : IApplePushNotificationService
    {
        private readonly HttpClient _client;
        private readonly MyApnSettings _myApnSettings;

        public ApplePushNotificationService(IOptions<MyApnSettings> myApnSettings)
        {
            _client = new HttpClient();
            _myApnSettings = myApnSettings.Value;
        }

        public async void SendPushForNewChatMessage(ChatUser chatUser, ChatUser sender)
        {
            try
            {
                if(chatUser.DeviceToken != null)
                {
                    var notification = new AppleNotification
                    {
                        Aps = new AppleNotification.ApsPayload
                        {
                            AlertBody = $"New Message from {sender.UserName}",
                            Sound = "default"
                        }
                    };

                    var deviceToken = chatUser.DeviceToken;
                    var settings = new ApnSettings
                    {
                        AppBundleIdentifier = _myApnSettings.APPIdentifier,
                        P8PrivateKey = _myApnSettings.PrivateKey,
                        P8PrivateKeyId = _myApnSettings.P8Key,
                        TeamId = _myApnSettings.TeamId,
                        ServerType = _myApnSettings.Server == "Production" ? ApnServerType.Production : ApnServerType.Development
                    };
                    var apn = new ApnSender(settings, _client);
                    await apn.SendAsync(notification, deviceToken);
                }
            }
            catch(Exception e)
            {
                throw new AppException($"Could not update device token {e.Message}");
            }
        }
    }
}