using Golf.Domain.Shared.Golfer.UserSetting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GolfersController.Requests.Setting
{
    public class UserSettingRequest
    {
        public NotificationSetting NotificationSetting = new NotificationSetting();
        public VideoSetting VideoSetting = VideoSetting.AutoWithWifi;
        public LanguageSetting LanguageSetting = LanguageSetting.vi;
    }
}
