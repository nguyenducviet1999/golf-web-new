using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Golf.Domain.Base;
using Golf.Domain.GolferData;

namespace Golf.Domain.Courses
{
    public class Course : IEntityBase
    {
        public Guid MainVersionID { get; set; }
        [ForeignKey("Location")]
        public Guid LocationID { get; set; }
        public virtual Location Location { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }
        public virtual Golfer Owner { get; set; }

        public string Cover { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PaymentType { get; set; }
        [Column(TypeName = "jsonb")]

        public List<int> Extensions { get; set; }
        public int Par { get; set; } = 0;
        [Column(TypeName = "jsonb")]
        public List<CourseInformation> MoreInformations { get; set; }
        public int TotalHoles { get; set; }
        [Column(TypeName = "jsonb")]
        public List<Tee> Tees { get; set; }
        [Column(TypeName = "jsonb")]
        public List<CourseHole> CourseHoles { get; set; }
        public string PhotoNames { get; set; }
        public int Version { get; set; }
        public bool IsConfirmed { get; set; } = false;
    }
}
