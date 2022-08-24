using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Notifications
{
    public static class NotificationContent
    {
        //friemd 
        public const string SendAddFriendRequest = "send a friend request to you";
        public const string ConfirmAddFriendRequest = "accepted your friend request";
        //post
        public const string TagUserInPost = "taged you in a post";
        public const string MentionsUserComment = "mentions you in a comment";
        public const string CommentYousTagPost = "commented on a post that you are tagged in";
        public const string CommentYousPost = "commented on your post";
        public const string LikeYourComment = "liked your comment";
        public const string LikeYourPost = "liked your post";
        public const string LikeYourTagPost = "liked your tag post";
        public const string RepliedYourComment = "replied to your comment";
        public const string SharePost = "shared your post";
        //scorecard
        public const string PlayWith = "You played with";
        public const string InCourse = "in Course";
        public const string EnterYourScorecard = "entered your scorecard.Please confirm and post your scorecard";
        public const string ConfirmScorecard = "have confirmed your game result is correct";
        public const string ConfirmIncorrectScorecard = "have confirmed your game result is incorrect";
        public const string InviteFriendConfirmscorecard = "invited you to confirm their scorecard";
        public const string ReportScorecard = "reported an inappropriate scorecard";
        //grroup
        public const string AddGroupMember = "added you to the group";
        public const string InviteGroupMember = "invited you to join the group";
        public const string SendGroupMemberRequest = "send request to join group";
        public const string RemoveGroupMember = "removed you from";
        public const string DeleteGroup = "group you joined has been deleted";
        public const string RejectModerator = "removed your moderator permission into";//chưa xong
        public const string ConfirmGroupMemberRequest = "confirm your request to join group";
        public const string GroupMemberPostNews = "posted in";
        public const string AddGroupModerator = "added you as the Moderator of";
        public const string AddGroupAdmin = "added you as the Admin of";
        //tounament
        public const string SendTournamentMemberRequest = "send request to join tournament";
        public const string SendTournamentRequest = "send request to add tournament";
        public const string ConfirmGroupMember = "approved your request to join";  
        public const string ConfirmTournamentMember = "approved your request to join";  
        public const string ConfirmTournamentFirst = "Your request to create";  
        public const string ConfirmTournamentLast = "has been accepted";  
        public const string RejectTournament = "rejected your request to add";
        public const string DeleteTournament = "tournament you participated in has been deleted";
        public const string RejectTournamentMember = "rejected your request to join";
        //event
        public const string InviteFriend = "invited you to join the match schedule at";

    }

}
