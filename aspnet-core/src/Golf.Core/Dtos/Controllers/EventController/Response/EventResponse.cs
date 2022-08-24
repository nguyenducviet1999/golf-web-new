using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.EventController.Response
{
    public class EventResponse
    {
        public Guid ID { get; set; }//định danh
        public string CourseName { get; set; }//tên sân
        public Guid CourseID { get; set; }//tên sân
        public DateTime Time { get; set; }//thời điểm
        public string Address { get; set; }//địa chỉ
        public MinimizedGolfer Owner { get; set; }//người tạo
        public int TotalHoles { get; set; }//số hố
        public string Description { get; set; }//mô tả
        public int MaxMembers { get; set; }// só luowjgn tối đa
        public bool PostNewfeed { get; set; }
        public bool InviteFriend { get; set; }
        public bool CourseBookingStatus { get; set; }
        public List<MinimizedGolfer> Members { get; set; } = new List<MinimizedGolfer>();//thành viên
        public EventPrivacy Privacy { get; set; }//phạm vi
        public Guid? ConvesationID { get; set; }//trò chuyện
        public bool Joined { get; set; }// đã tham gia hay chưa
        public bool AllowViewing { get; set; } = true;// 


    }
}
