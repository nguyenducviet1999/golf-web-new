using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class DateBookingInfo
    {
        public DateTime Date { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsPromotion { get; set; }


    }
}
