using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Notifications
{
    public static class ObjectType
    {
        public const int User = 0;
        public const int Group = 1;
        public const int Post = 2;
        public const int Comment = 3;
        public const int GroupPost = 4;
        public const int Course = 5;
        public const int Event = 6;
        public const int Scorecard = 7;
        public const int Tournament = 8;
    }
    public static class NotificationType
    {
        public const int AddFriend = 1;
        public const int Share = 2;
        public const int Comment = 3;
        public const int Like = 4;
        public const int Event = 5;
        public const int CommentTag = 6;
        public const int Checkin = 7;
        public const int Group = 8;
        public const int PostTag = 9;
        public const int ConfirmScorecarrds = 10;
        public const int EnterScorecarrds = 11;
        public const int Tournament = 12;

    }
}
