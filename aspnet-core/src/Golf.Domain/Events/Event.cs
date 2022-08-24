using Golf.Domain.Base;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Event;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Events
{
    public class Event : IEntityBase
    {
        public virtual Course Course { get; set; }
        [ForeignKey("Course")]
        public Guid CourseID { get; set; }
        public DateTime Time { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }
        public virtual Golfer Owner { get; set; }
        public int TotalHoles { get; set; }
        public string Description { get; set; }
        public bool PostNewfeed { get; set; }
        public bool InviteFriend { get; set; }
        public int MaxMembers { get; set; }
        public bool CourseBookingStatus { get; set; }
        public string MemberIDs { get; set; } ="";
        public EventPrivacy Privacy { get; set; }
        public Guid? ConvesationID { get; set; }

        public void AddMemberID(Guid guid)
        {
            if(this.MemberIDs=="")
            {
                this.MemberIDs += guid.ToString();
            }
            else
            {
                this.MemberIDs += "," + guid.ToString();
            }
        }
        public void RemoveMemberID(Guid guid)
        {
            if(this.MemberIDs.Contains(guid.ToString()))
            {
                this.MemberIDs= this.MemberIDs.Replace("," + guid.ToString(), "");
                this.MemberIDs= this.MemberIDs.Replace(guid.ToString() + ",", "");
            }    
        }
        public List<Guid> GetMemberIDs()
        {
            List<Guid> result = new List<Guid>();
           var tmp= this.MemberIDs.Split(",");
            foreach(var i in tmp)
            {
                if (i == "") continue;
                result.Add(new Guid(i));
            }
            return result;
        }

    }
   
}