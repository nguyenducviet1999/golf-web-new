using Golf.Core.Dtos.Controllers.EventController.Request;
using Golf.Core.Dtos.Controllers.EventController.Response;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Event;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.Events;
using Microsoft.AspNetCore.Mvc;
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
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/<EventsController>
        /// <summary>
        /// Lấy dự kiện theo định danh
        /// </summary>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <returns></returns>
        [HttpGet("{eventID}")]
        public ActionResult<EventResponse> Get(Guid eventID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.Get(golfer.Id, eventID));
        }

        /// <summary>
        /// Lấy các sự kiện đã tham gia
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{eventFilter}/{startIndex}")]
        public ActionResult<List<EventResponse>> MyEvent(EventFilterByStatus eventFilter, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.GetEventByFilter(golfer.Id, eventFilter, startIndex));
        }
        [HttpGet("GetEventByFilter/{startIndex}")]
        public ActionResult<List<EventResponse>> FilterEvent([FromQuery] EventFilterByStatus? eventFilter, [FromQuery] EventPrivacy? eventPrivacy, [FromQuery] DateTime? date, [FromQuery] string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.FilterEvent(golfer.Id, eventFilter, eventPrivacy, date, searchKey==null?"":searchKey.Trim().ToLower(), startIndex));
        }
        /// <summary>
        /// tìm kiếm sự kiện theo tên sự kiện hoặc tên đơn vị tổ chức
        /// </summary>
        /// <param name="searchKey"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Search/{eventFilterByPrivacy}/{startIndex}")]
        public ActionResult<List<EventResponse>> SearchEvent([FromQuery] string searchKey, EventFilterByPrivacy eventFilterByPrivacy, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.SearchEvent(golfer.Id, eventFilterByPrivacy, searchKey == null ? "" : searchKey.Trim().ToLower(),startIndex));
        }
        /// <summary>
        /// lọc sự kiện theo ngày
        /// </summary>
        /// <param name="date"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("FilterEventByDate/{date}/{startIndex}")]
        public ActionResult<List<EventResponse>> FilterEventByDate(DateTime date, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.FilterEventByDate(golfer.Id, date, startIndex));
        }

        /// <summary>
        /// Tạo sự kiện
        /// </summary>
        /// <param name="addEventRequest"></param>
        /// <returns></returns>
        // GET api/<EventsController>/5
        [HttpPost]
        public ActionResult<EventResponse> AddEvent([FromBody] AddEventRequest addEventRequest)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.Add(golfer.Id, addEventRequest).Result);
        }

        /// <summary>
        /// Sửa sự kiện
        /// </summary>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <param name="addEventRequest">Dữ liệu thay đổi</param>
        /// <returns></returns>
        // POST api/<EventsController>
        [HttpPut("{eventID}")]
        public ActionResult<EventResponse> Edit(Guid eventID, [FromBody] AddEventRequest addEventRequest)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.Edit(golfer.Id, eventID, addEventRequest).Result);
        }
        [HttpPut("{eventID}/AddConversation")]
        public ActionResult<Guid> AddConversation(Guid eventID,[FromQuery]string conversationName)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];

            return Ok(_eventService.AddConversation(golfer.Id, eventID,conversationName));
        }

        /// <summary>
        /// tham gia sự kiện
        /// </summary>
        /// <param name="eventID"></param>
        /// <returns></returns>
        // PUT api/<EventsController>/5
        [HttpPut("{eventID}/Join")]
        public ActionResult<bool> JoinEvent(Guid eventID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.JoinEvent(golfer.Id, eventID));
        }

        /// <summary>
        /// Xóa sự kiện
        /// </summary>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <returns></returns>
        // DELETE api/<EventsController>/5
        [HttpDelete("{eventID}")]
        public ActionResult<bool> DeleteEvent(Guid eventID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.Delete(golfer.Id, eventID));
        }

        /// <summary>
        /// Rời sự kiện  
        /// </summary>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <returns></returns>
        [HttpPut("{eventID}/ExitEvent")]
        public ActionResult<bool> ExitEvent(Guid eventID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_eventService.ExitEvent(golfer.Id, eventID));
        }


    }
}
