using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Golfer.UserSetting
{
    public class UserSetting
    {
        public NotificationSetting NotificationSetting = new NotificationSetting();
        public VideoSetting VideoSetting = VideoSetting.AutoWithWifi;
        public LanguageSetting LanguageSetting = LanguageSetting.vi;
        public List<GroupSetting> GroupSettings = new List<GroupSetting>();
    }
}
