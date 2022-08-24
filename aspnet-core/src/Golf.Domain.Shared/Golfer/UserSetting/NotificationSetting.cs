using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Golfer.UserSetting
{
    public class NotificationSetting
    {
        public bool Comment=true;//thông báo bình luận bài viết liên quan đến bạn
        public bool Tag=true;//thông báo gắn thẻ hoặc nhắc đến trong bình luận 
        public bool MakeFriend=true;//Thông báo về yêu cầu kết bạn hoặc xác nhận kết bạn
        public bool FriendAchievements=true;//Thông báo về thành tích bạn bè

    }
}
