using Golf.Domain.Notifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.EntityFrameworkCore.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>
    {
        public NotificationRepository(GolfDbContext context) : base(context)
        {

        }
    }
}
