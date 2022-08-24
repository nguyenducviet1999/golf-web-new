using Golf.Domain.Base;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Golf.Domain.Bookings
{
    public class Product : IEntityBase
    { 
        [ForeignKey("Course")]
        public Guid CourseID { get; set; }
        public virtual Course Course { get; set; }
        public double Price { get; set; }
        public int  MaxPlayer { get; set; }
        public double Promotion { get; set; } = 0;
        public double MembershipPromotion { get; set; } = 0;
        public bool FullBooking { get; set; } = false;
        public DateTime Date { get; set; }
        public TimeSpan TeeTime { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "jsonb")]
        public List<int> LisExtensionID { get; set; } = new List<int>();

    }
}
