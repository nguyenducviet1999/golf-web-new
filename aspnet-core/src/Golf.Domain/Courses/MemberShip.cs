using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Course;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Courses
{
    public class MemberShip : IEntityBase
    {
       
     
        [ForeignKey("Golfer")]
        public Guid GolferID { get; set; }
        public virtual Golfer Golfer { get; set; }
        public CourseMemberShipStatus Status { get; set; }
        public int Type { get; set; }
    }
}
