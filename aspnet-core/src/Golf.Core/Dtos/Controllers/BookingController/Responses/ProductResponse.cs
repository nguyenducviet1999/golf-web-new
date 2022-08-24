using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class ProductResponse
    {
        public Guid ID { get; set; }
        public Guid CourseID { get; set; }
        public double Price { get; set; }
        public int  MaxPlayer { get; set; }
        public double MembershipPromotion { get; set; } = 0;
        public bool IsBooking { get; set; } = false;

        public double Promotion { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TeeTime { get; set; }
     
        public List<CourseExtension> LisExtension { get; set; } = new List<CourseExtension>();

    }
}
