using AutoMapper;
using Golf.Core.Dtos.Controllers.NotificationController.Requests;
using Golf.Core.Dtos.Controllers.NotificationController.Respone;
using Golf.Domain.Notifications;
using Golf.Domain.GolferData;
using Golf.Services.Notifications;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Golf.HttpApi.Host.Helpers;
namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class NotificationController : ControllerBase

    {
        private readonly NotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(NotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách thông báo golfer
        /// </summary>
        /// <returns></returns>
        [HttpGet("MyNotification")]
        public ActionResult<List<NotificationResponse>> Get()
        { 
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_notificationService.GetMyNotifiction(golfer.Id, 0));
        }
        /// <summary>
        /// Đếm sô tin nhắn chưa đọc
        /// </summary>
        /// <returns></returns>
        [HttpGet("CountUnRead")]
        public ActionResult<int> CountUnReadNotification()
        { 
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_notificationService.UnReadNotification(golfer.Id));
        }
        /// <summary>
        /// Xem thông báo
        /// </summary>
        /// <param name="notificationID">Định danh thông báo</param>
        /// <returns></returns>
        [HttpPut("{notificationID}/view")]
        public ActionResult<bool> ViewNotification(Guid notificationID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_notificationService.View(golfer.Id, notificationID));
        }
        /// <summary>
        /// Đánh dấu đọc tất cả thông báo
        /// </summary>
        /// <returns></returns>
        [HttpPut("viewall")]
        public ActionResult<bool> ViewAllNotification()
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_notificationService.ViewAll(golfer.Id));
        }

        /// <summary>
        /// XÓa thông báo
        /// </summary>
        /// <param name="notificationID"></param>
        /// <returns></returns>
        // DELETE api/<NotificationController>/5
        [HttpDelete("{notificationID}")]
        public ActionResult<bool> Delete(Guid notificationID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_notificationService.Delete(golfer.Id, notificationID));
        }
    }
}
