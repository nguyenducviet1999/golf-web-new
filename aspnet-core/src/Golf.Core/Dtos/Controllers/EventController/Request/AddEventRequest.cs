using Golf.Domain.Events;
using Golf.Domain.Shared.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.EventController.Request
{
    public class AddEventRequest
    {
   
        public Guid CourseID { get; set; }
        public DateTime Time { get; set; }
        public int TotalHoles { get; set; }
        public string Description { get; set; }
        public int MaxMembers { get; set; }
        public bool PostNewfeed { get; set; }
        public bool InviteFriend { get; set; }
        public bool CourseBookingStatus { get; set; }
        public EventPrivacy Privacy { get; set; }
        //public Guid? ConvesationID { get; set; }
    }
}
