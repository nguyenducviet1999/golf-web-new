using Golf.Core.Common.Golfer;
using Golf.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.NotificationController.Respone
{
   public class NotificationResponse
    {
        public Guid ID { get; set; }
        public string Content { get; set; }
        public string? Image { get; set; }
        public List<NotificationObject> Objects { get; set; } = new List<NotificationObject>();
        public NotificationObject ReferObject { get; set; } = new NotificationObject();
        public bool IsViewed { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Type { get; set; }



    }
  
}
