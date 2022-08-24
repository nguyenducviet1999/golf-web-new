using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class SearchCourseBookingRespone
    {
        public SearchCourseBookingRespone(){}
        public SearchCourseBookingRespone(Guid id,string name,string address)
        {
            ID = id;
            Name = name;
            Address = address;
        }
        public Guid ID;
        public string  Name;
        public string Address;

    }
}
