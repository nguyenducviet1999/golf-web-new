using Golf.Core.Dtos.ConvarsationsController.Respone;
using Golf.Core.Dtos.HubSinalR;
using Golf.Domain.GolferData;
using Golf.Domain.Messages;
using Golf.Domain.Shared.AccessToken;
using Golf.Domain.Shared.Messages;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{


    public class HubSignalR : Hub
    {
        public static List<User> ConnectedUsers = new List<User>();
        private readonly ConversationService _conversationService;
        private readonly ConversationRepository _conversationRepository;
        private readonly AccessTokenHandler _accessTokenHandler;
        private readonly GolferRepository _golferRepository;
        public HubSignalR(GolferRepository golferRepository, AccessTokenHandler accessTokenHandler, ConversationService conversationService, ConversationRepository conversationRepository)
        {
            _accessTokenHandler = accessTokenHandler;
            _conversationRepository = conversationRepository;
            _conversationService = conversationService;
            _golferRepository = golferRepository;
        }
        /// <summary>
        /// Lưu và gửi tin nhắn đến máy khách 
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        public async void SendMessage(Guid toConversationID, string message,List<String>? photoNames)
        {
            //var golfer = (Golfer)Context. .HttpContext.Items["Golfer"];
            string fromconnectionid = Context.ConnectionId;
            if (fromconnectionid == null)
            {
                return;
            }
            Guid fromUserId = new Guid(Context.Items["id"].ToString()); // ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault();
            if (fromUserId == Guid.Empty)
            {
                return;
            }

            var senderID = new Guid(fromUserId.ToString());
            var conversation = _conversationRepository.Get(toConversationID);
            if (conversation == null)
            {
                return;
            }

            var t = _conversationService.AddMessagess(senderID, toConversationID, message,photoNames);//Lưu tin nhắn
            if (t != true) return;
            var receiverIDs = conversation.GetGolferIDs();
            foreach (var i in receiverIDs)
            {
                try
                {
                    if(i!=fromUserId)
                    {
                        await Clients.Group(i.ToString()).SendAsync("receiverMessage", fromUserId, message,conversation.ID,photoNames); ;//Gửi tin nhắn đến người nhận
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }

        public async void ReadMessage(Guid toConversationID)
        {
            string fromconnectionid = Context.ConnectionId;
            if (fromconnectionid == null)
            {
                return;
            }
            Guid fromUserId = ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault();
            if (fromUserId == Guid.Empty)
            {
                return;
            }

            var senderID = new Guid(fromUserId.ToString());
            var conversation = _conversationRepository.Get(toConversationID);
            if (conversation == null && conversation.GolferIDs.Contains(fromconnectionid) == false)
            {
                return;
            }
            var receiverIDs = conversation.GetGolferIDs();
            foreach (var i in receiverIDs)
            {
                //await Clients.Client(ToUser.ConnectionId).SendAsync("receiverMessage", tmp.UserID, message);//Gửi tin nhắn đến người nhận
                //await Clients.Client(tmp.ConnectionId).SendAsync("sent", tmp.UserID, message);//gửi thông báo đã gửi thành công đến 
                if (i != fromUserId)
                {
                    await Clients.Group(i.ToString()).SendAsync("readMessage", fromUserId, conversation.ID);//Gửi tin nhắn đến người nhận
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
           await Groups.RemoveFromGroupAsync(Context.ConnectionId, (string)Context.Items["id"]);
            await base.OnDisconnectedAsync(exception);
        }
        public override async Task OnConnectedAsync()
        {
            var accessToken = Context.GetHttpContext().Request.Query["access_token"];
            Context.Items["id"] = _accessTokenHandler.Handle(accessToken);
           await Groups.AddToGroupAsync(Context.ConnectionId, (string)Context.Items["id"]);
            //Context.User.Identity.id
            await base.OnConnectedAsync();
        }

        public void SendUserTypingRequest(Guid toUserId)
        {
            string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();
            List<User> ToUsers = ConnectedUsers.Where(x => x.UserID == toUserId).ToList();
            foreach (var ToUser in ToUsers)
            {
                // send to                                                                                            
                // Clients.Client(ToUser.ConnectionId).ReceiveTypingRequest(strfromUserId);
            }
        }
        public static string GetConnectId(Guid userID)
        {
            var Connecter = ConnectedUsers.Where(x => x.UserID == userID);
            if (Connecter.Count() == 0)
                return "";
            return Connecter.FirstOrDefault().ConnectionId;
        }

    }
}
