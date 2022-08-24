using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
   public class SearchBookingResponse
    {
        public List<SearchLocationRespone> SearchLocationRespones= new List<SearchLocationRespone>();
        public List<SearchCourseBookingRespone> SearchCourseBookingRespones= new List<SearchCourseBookingRespone>();
    }
}
