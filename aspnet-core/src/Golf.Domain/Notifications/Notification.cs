using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Notifications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Notifications
{
    public class Notification : IEntityBase
    {

        [ForeignKey("Golfer")]
        public Guid golferID { get; set; }
        public virtual Golfer Golfer { get; set; }
        public string Content { get; set; }
        [Column(TypeName = "jsonb")]
        public List<NotificationObject> Objects { get; set; } = new List<NotificationObject>();
        [Column(TypeName = "jsonb")]
        public NotificationObject ReferObject { get; set; } = new NotificationObject();
        public bool IsViewed { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int Type { get; set; }
       
    }
    public class NotificationObject
    {
        public string? Image { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public Guid ID { get; set; }
        

    }

}
