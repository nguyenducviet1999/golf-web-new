using Golf.Core.Dtos.Controllers.ConvarsationsController.Requests;
using Golf.Core.Dtos.Controllers.ConvarsationsController.Respone;
using Golf.Core.Dtos.ConvarsationsController.Respone;
using Golf.Domain.GolferData;
using Golf.Domain.Messages;
using Golf.Domain.Shared.Messages;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.messages;
using Golf.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private readonly ConversationService _conversationService;
        private readonly IHubContext<HubSignalR> _hubContext;
        public ConversationController(ConversationService conversationService, IHubContext<HubSignalR> hubContext)
        {
            _conversationService = conversationService;
            _hubContext = hubContext;
        }
        // GET: api/<ConversationController>
        //[HttpGet("{sender}/{receirver}")]
        //public ActionResult<ConversationResponse> Get(Guid sender, Guid receirver)
        //{
        //    return _conversationService.GetMessagess(sender, receirver, 0);


        //}
        /// <summary>
        /// Tạo cuộc trò chuyện mới
        /// </summary>
        /// <param name="sender">ID thành viên cuộc trò chuyện</param>
        /// <param name="receirver">ID thành viên cuộc trò chuyện</param>
        /// <returns>Cuộc trò chuyện</returns>
        //[HttpPost("{sender}/{receirver}")]
        //public ActionResult<Conversation> post(Guid sender, Guid receirver)
        //{

        //    Conversation t = new Conversation();
        //    t.AddGolferID(sender);
        //    t.AddGolferID(receirver);
        //    _conversationService._conversationRepository.Add(t);
        //    return t;


        //}
        /// <summary>
        /// Lấy danh sách các cuộc trò chuyện
        /// </summary>
        /// <param name="startIndex">Số thứ tự phần tử dầu tiên</param>
        /// <returns>Danh sách cuộc trò chuyện </returns>
        [HttpGet("Conversations/{startIndex}")]
        public ActionResult<List<ConversationResponse>> GetConversations(int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _conversationService.GetConversations(currentGolfer.Id, startIndex).Result;
        }

        /// <summary>
        /// đếm số lượng tin nhấn chưa đọc
        /// </summary>
        /// <returns>Số luowjgn tin nhấn chưa đọc</returns>
        [HttpGet("Conversations/CountUnreadMessaages")]
        public ActionResult<int> CountUnreadMessaages()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _conversationService.CountUnReadMesaages(currentGolfer.Id);
        }

        /// <summary>
        /// Lấy nội dung cuộc trò chuyện 
        /// </summary>
        /// <param name="receiverID">ID người nhận</param>
        /// <param name="startIndex">Số thứ tự tin nhắn bắt đầu</param>
        /// <returns></returns>
        [HttpGet("Receiver/{receiverID}/Messages/{startIndex}")]
        public ActionResult<ConversationResponseDetail> GetMessagesByFiendID(Guid receiverID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return _conversationService.GetMessagessByFiendID(currentGolfer.Id, receiverID, startIndex).Result;
        }

        /// <summary>
        /// Lấy các tin nhắn trong cuộc trò chuyện
        /// </summary>
        /// <param name="conversationID">định danh cuộc trò chuyện</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Conversation/{conversationID}/Messages/{startIndex}")]
        public ActionResult<ConversationResponseDetail> GetMessagesByConversationID(Guid conversationID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_conversationService.GetMessagessByConversationID(currentGolfer.Id, conversationID, startIndex).Result);
        }

        [HttpPut("Conversation/{conversationID}/member")]
        public ActionResult AddMember(Guid conversationID, [FromBody] List<Guid> golferIDs)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_conversationService.AddUser(currentGolfer.Id, conversationID, golferIDs));
        }

        [HttpPut("Conversation/GroupChat")]
        public ActionResult<ConversationResponseDetail> AddGroupChat([FromBody] GroupChatRequest groupChatRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_conversationService.AddGroupChat(currentGolfer.Id, groupChatRequest.GolferIDs, groupChatRequest.ConversationName));
        }
        [HttpPost("Conversation/Messages")]
        public async Task<ActionResult<bool>> SendMessages([FromForm] SendMessageRequest sendMessageRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(await _conversationService.SendMessage(currentGolfer.Id, sendMessageRequest));
        }

        /// <summary>
        /// Gửi tin nhắn(API chỉ để thử ko dùng để ghép)
        /// </summary>
        /// <param name="conversationID">định danh cuộc trò chuyện</param>
        /// <param name="mess"></param>
        /// <returns></returns>
        //[HttpPost("Conversation/{conversationID}/Messages/{mess}")]
        //public ActionResult<bool> SendMess(Guid conversationID, string mess)
        //{
        //    var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
        //    var result = _conversationService.AddMessagess(currentGolfer.Id, conversationID, mess);
        //    return result;
        //}

    }
}
