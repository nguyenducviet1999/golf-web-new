using Golf.Domain.Notifications;
using Golf.Domain.Shared.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.NotificationController.Requests
{
    public class NotificationRequest
    {
        public Guid golferID { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }

        public List<NotificationObject> Objects { get; set; } = new List<NotificationObject>();
        public NotificationObject ReferObject { get; set; } = new NotificationObject();

    }
   
}
