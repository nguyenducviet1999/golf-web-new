using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class CourseBookingResponse
    {
        public BookingResponse CourseInfor { get; set; } = new BookingResponse();
        public List<DateBookingInfo> DateBookingInfos { get; set; } = new List<DateBookingInfo>();
    }
}
