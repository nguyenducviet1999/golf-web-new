using AutoMapper;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.ConvarsationsController.Requests;
using Golf.Core.Dtos.Controllers.ConvarsationsController.Respone;
using Golf.Core.Dtos.ConvarsationsController.Respone;
using Golf.Core.Exceptions;
using Golf.Domain.Messages;
using Golf.Domain.Resources;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Messages;
using Golf.Domain.Shared.Resources;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.messages
{
   public class ConversationService
    {
        public readonly GolferRepository _golferRepository;
        public readonly PhotoService _photoService;
        public readonly GolferService _golferService;
        public readonly MessageRepository _messageRepository;
        public readonly ConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<HubSignalR> _hubContext;
        private readonly DatabaseTransaction _databaseTransaction;
        public ConversationService(PhotoService photoService, DatabaseTransaction databaseTransaction, IHubContext<HubSignalR> hubContext, MessageRepository messageRepository, GolferService golferService, GolferRepository golferRepository, IMapper mapper, ConversationRepository conversationRepository)
        {
            _photoService = photoService;
            _databaseTransaction = databaseTransaction;
            _hubContext = hubContext;
            _messageRepository = messageRepository;
            _golferRepository = golferRepository;
            _mapper = mapper;
            _conversationRepository = conversationRepository;
            _golferService = golferService;
        }

        public ConversationResponseDetail GetConversationResponseDetail(Guid currentUserID, Conversation conversation, int startIndex)
        {
            ConversationResponseDetail respone = new ConversationResponseDetail();
            if (conversation.GetGolferIDs().Contains(currentUserID) == false)
            {
                throw new ForbiddenException("Not allow!");
            }
            foreach (var i in conversation.GetGolferIDs())
            {
                var golfer = _golferRepository.GetMinimizedGolfer(i);
                if (golfer != null)
                    respone.Golfers.Add(golfer);
            }
            respone.ID = conversation.ID;
            if (conversation.Name != null)
            {
                respone.Name = conversation.Name;
            }
            else
            {
                respone.Name = respone.Golfers.Find(c => c.ID != currentUserID).FullName;
            }
            if (conversation.ReaderIDs.Contains(currentUserID) == false)
            {
                conversation.ReaderIDs.Add(currentUserID);
                _conversationRepository.Update(conversation);
            }
            respone.messages = _messageRepository.Find(m => m.ConversationID == conversation.ID).OrderByDescending(m => m.Time).Skip(startIndex).Take(Const.PageSize).OrderBy(m => m.Time).ToList();
                return respone;
        }
        public ConversationResponse GetConversationResponse(Guid currentUserID, Conversation conversation)
        {
            ConversationResponse respone = new ConversationResponse();
            respone.ID = conversation.ID;
            var receiverIDs = conversation.GetReceivers(currentUserID);
            var receivers = _golferRepository.Find(c => receiverIDs.Contains(c.Id)).ToList();
            foreach (var j in receivers)
            {
                respone.Receivers.Add(_mapper.Map<MinimizedGolfer>(j));
            }
            if (conversation.Name == null)
            {
                if (conversation.GetGolferIDs().Count() == 2)
                {
                    respone.Name = respone.Receivers.First().FullName;
                }
                else
                {
                    respone.Name = "";
                    foreach (var t in respone.Receivers)
                    {
                        respone.Name += t.FullName + ", ";
                    }
                }
            }
            else
            {
                respone.Name = conversation.Name;
            }
            if (conversation.Image == null&& respone.Receivers.Count()>0)
            {
                respone.Image = respone.Receivers.First().Avatar;
            }
            else
            {
                respone.Image = conversation.Image;
            }
            var mess = _messageRepository.Find(m => m.ConversationID == conversation.ID).OrderByDescending(m=>m.Time).FirstOrDefault();
        
            if (mess == null)
            {

                respone.IsRead = conversation.read(currentUserID);
                respone.LastMessages = "";
                respone.LastTime = conversation.CreatedDate;
                respone.OwnerIDLastMessages = currentUserID;
            }
            else
            {
                respone.OwnerIDLastMessages = mess.SenderID;
                respone.IsRead = conversation.read(currentUserID);
                respone.LastMessages = mess != null ? mess.Content : "";
                respone.LastTime = mess != null ? mess.Time : conversation.CreatedDate;
            }
            return respone;
        }
        public async Task<ConversationResponseDetail>  GetMessagessByFiendID(Guid currentUserID, Guid friendID,int startIndex)
        {
            ConversationResponseDetail respone = new ConversationResponseDetail();
            var result = _conversationRepository.Find(cv =>cv.GolferIDs.Contains(currentUserID.ToString())&& cv.GolferIDs.Contains(friendID.ToString())&&cv.Name==null).FirstOrDefault();
            if (result == null)
            {
                Conversation conversation = new Conversation();
                conversation.AddGolferID(currentUserID);
                conversation.AddGolferID(friendID);
                _conversationRepository.Add(conversation);
                respone = this.GetConversationResponseDetail(currentUserID, conversation, startIndex);  
            }
            else
            {
                respone = this.GetConversationResponseDetail(currentUserID, result, startIndex);
            }
            return respone;
        }

         public async Task<ConversationResponseDetail>  GetMessagessByConversationID(Guid currentUserID, Guid conversationID,int startIndex)
        {
            ConversationResponseDetail respone = new ConversationResponseDetail();
            var golfers = _golferRepository.GetMinimizedGolfer(currentUserID);
            var result = _conversationRepository.Get(conversationID);
            if (result == null)
            {
                throw new NotFoundException("Not found conversation");
            }
            else
            {
                respone = this.GetConversationResponseDetail(currentUserID, result, startIndex);
            }
                return respone;
        }
        public bool AddUser(Guid currentID,Guid conversationID, List<Guid> userIDs)
        {
            foreach(var i in userIDs)
            {
                var user = _golferRepository.Get(i);
                if (user == null)
                    throw new NotFoundException("Not found user");
            }    
            var conversation = _conversationRepository.Get(conversationID);
            if(conversation==null)
                throw new NotFoundException("Not found conversation");
            if (conversation.GetGolferIDs().Contains(currentID) == false||conversation.Name==null)
                return false;
            if(conversation.Name!=null)
            {
                foreach (var i in userIDs)
                {
                    if(conversation.GetGolferIDs().Contains(i)==false)
                    {
                        conversation.AddGolferID(i);
                    }    
                }
                _conversationRepository.UpdateEntity(conversation);
                return true;
            }
            return false;
        }     
        public int CountUnReadMesaages(Guid currentID)
        {
            int sum = 0;
            var conversations = _conversationRepository.Find(c => c.GolferIDs.Contains(currentID.ToString())&&(c.Name != null||c.IsEmpty==false));
            foreach(var i in conversations)
            {
                if(i.read(currentID)==false)
                { sum++; }    
            }
            return sum;
        }
        public async Task<List<ConversationResponse>> GetConversations(Guid currentUserID, int startIndex)
        {
            List<ConversationResponse> result = new List<ConversationResponse>();
           // var golfers = _golferRepository.Entities.ToList();
           var tmp= _conversationRepository.Find(c => c.GolferIDs.Contains(currentUserID.ToString())&& (c.Name != null || c.IsEmpty == false)).OrderByDescending(c=>c.ModifiedDate).Skip(startIndex).Take(Const.PageSize);
            foreach(var i in tmp)
            {
                result.Add(this.GetConversationResponse(currentUserID,i));
            }
            return result.OrderByDescending(m=>m.LastTime).ToList();
        }

       
        //public  bool AddMessagess(Guid currentUserID, Guid conversationID, string txtMessage, MessagesType messagesType)
        //{
        //    var conversation = _conversationRepository.Get(conversationID);
            
        //    if(conversation==null)
        //    {
        //        throw new NotFoundException("Not found conversation");
        //    }
          
        //    if (conversation.GetGolferIDs().FindAll(g => g == currentUserID).Count() == 0)
        //    {
        //        throw new BadRequestException("Not allow!");
        //    }

        //    Message message = new Message();
        //    message.SenderID = currentUserID;
        //    message.Content = txtMessage;
        //    message.Type = messagesType;
        //    message.ConversationID = conversationID;
        //    if(conversation.IsEmpty==true)
        //    {
        //        conversation.IsEmpty = false;
        //    }    
        //    conversation.ReaderIDs=new List<Guid>();
        //    conversation.ReaderIDs.Add(currentUserID);
        //    _conversationRepository.Update(conversation);
        //    return true;
        //}
        public  bool AddMessagess(Guid currentUserID, Guid conversationID, string txtMessage, List<string>? photoNames)
        {
            var conversation = _conversationRepository.Get(conversationID);
            if(conversation==null)
            {
                throw new NotFoundException("Not found conversation");
            }
            if (conversation.GetGolferIDs().FindAll(g => g == currentUserID).Count() == 0)
            {
                throw new BadRequestException("Not allow!");
            }
            Message message = new Message();
            message.SenderID = currentUserID;
            message.Content = txtMessage;
            message.SetPhotoNames(photoNames);
            message.ConversationID = conversationID;
            _messageRepository.Add(message);
            if(conversation.IsEmpty==true)
            {
                conversation.IsEmpty = false;
            }    
            conversation.ReaderIDs=new List<Guid>();
            conversation.ReaderIDs.Add(currentUserID);
            _conversationRepository.Update(conversation);
            return true;
        } 
        public  bool SafeAddMessagess(Guid currentUserID, Guid conversationID, string txtMessage, List<string>? photoNames)
        {
            var conversation = _conversationRepository.Get(conversationID);
            if(conversation==null)
            {
                throw new NotFoundException("Not found conversation");
            }
            if (conversation.GetGolferIDs().FindAll(g => g == currentUserID).Count() == 0)
            {
                throw new BadRequestException("Not allow!");
            }
            Message message = new Message();
            message.SenderID = currentUserID;
            message.Content = txtMessage;
            message.SetPhotoNames(photoNames);
            message.ConversationID = conversationID;
            _messageRepository.SafeAdd(message);
            if(conversation.IsEmpty==true)
            {
                conversation.IsEmpty = false;
            }    
            conversation.ReaderIDs=new List<Guid>();
            conversation.ReaderIDs.Add(currentUserID);
            _conversationRepository.SafeUpdate(conversation);
            return true;
        }
       
        public bool DeleteMessagess(Guid currentUserID, Guid messageID )
        {
            var golfer = _golferRepository.Get(currentUserID);
            var mss = _messageRepository.Get(messageID);
            if (mss == null)
                throw new NotFoundException("Not found Message");
            var conversation = _conversationRepository.Get(mss.ConversationID);
            if (golfer == null || conversation == null)
            {
                throw new NotFoundException("Golfer isn't exit");
            }
            if (conversation.GolferIDs.Contains(golfer.Id.ToString()))
            {
                throw new Exception("This use wasn't joined");
            }
            _messageRepository.RemoveEntity(mss);
            return true;
        }

        public ConversationResponseDetail AddGroupChat(Guid currentUserID, List<Guid> golferIDs, string conversationName)
        {
            if (golferIDs.Count() <= 1)
                throw new BadRequestException("Not enough golfer");
            foreach(var i in golferIDs)
            {
                if (_golferRepository.Get(i) == null)
                    throw new NotFoundException("Not found golfer!");
            }
            Conversation conversation = new Conversation();
            conversation.Name = conversationName;
            conversation.AddGolferID(currentUserID);
            foreach (var i in golferIDs)
            {
                conversation.AddGolferID(i);
            }
            _conversationRepository.Add(conversation);
            return this.GetMessagessByConversationID(currentUserID,conversation.ID,0).Result;
        }
        public async Task<bool> SendMessage(Guid currentGolferID, SendMessageRequest sendMessageRequest)
        {
            List<Photo> photos = new List<Photo>();
            List<string> photoNames = new List<string>();

            _databaseTransaction.BeginTransaction();
            try
            {
                var conversation = _conversationRepository.Get(sendMessageRequest.ConversationID);
                if (conversation == null)
                {
                    return false;
                }
                if (conversation.GolferIDs.Contains(currentGolferID.ToString()) == false)
                    throw new ForbiddenException("Access Denied!");
                if(sendMessageRequest.PhotoFiles.Count()>0)
                {
                  photos= await _photoService.SavePhotos(currentGolferID, sendMessageRequest.PhotoFiles,PhotoType.Message);
                   photoNames = photos.Select(p => p.Name).ToList();
                }    
                
                var t = this.SafeAddMessagess(currentGolferID, sendMessageRequest.ConversationID, sendMessageRequest.Message, photoNames);//Lưu tin nhắn
                if (t != true) return false;
                var receiverIDs = conversation.GetGolferIDs();
                foreach (var i in receiverIDs)
                {
                    try
                    {
                        if (i != currentGolferID)
                        {
                            await _hubContext.Clients.Group(i.ToString()).SendAsync("receiverMessage", currentGolferID, sendMessageRequest.Message, conversation.ID, photoNames); ;//Gửi tin nhắn đến người nhận
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                await _databaseTransaction.Commit();
                return true;
            }
            catch(Exception e)
            {
                _photoService.DeletePhotos(photoNames);
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
    }
}
