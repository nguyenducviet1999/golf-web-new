using Golf.Domain.Bookings;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TransactionController.Respone
{
    public class TransactionResponse
    {
        public Guid ID { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TeeTime { get; set; }
        public string CourseName { get; set; }
        public Guid CourseID { get; set; }
        public string CourseAddress { get; set; }
        public double Price { get; set; }
        public string CourseImage { get; set; }
        public int  TotalPlayer { get; set; }
        public List<CourseExtension> LisExtension { get; set; } = new List<CourseExtension>();
        public string PromotionCode { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public string Description { get; set; }
        public List<int> MoreRequests { get; set; }

    }
}
