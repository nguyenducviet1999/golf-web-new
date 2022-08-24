using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Golfer.UserSetting
{
    public class GroupSetting
    {
        public Guid GroupID { get; set; }
        public NotificationGroupSetting NotificationGroupSetting = NotificationGroupSetting.All;
    }
    public enum NotificationGroupSetting
    {
        All,
        Friend,
        None
    }
}
