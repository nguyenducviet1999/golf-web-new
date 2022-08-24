using AutoMapper;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.NotificationController.Requests;
using Golf.Core.Dtos.Controllers.NotificationController.Respone;
using Golf.Core.Exceptions;
using Golf.Domain.Notifications;
using Golf.Domain.GolferData;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using Golf.Domain.Shared.Notifications;
using System.Threading.Tasks;
using SignalRChat.Hubs;
using Golf.Domain.Events;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Domain.Shared.Tuanament;
using Microsoft.AspNetCore.Identity;
using Golf.Domain.Shared.Golfer;

namespace Golf.Services.Notifications
{
    public class NotificationService
    {
        private readonly NotificationRepository _notificationRepository;
        private readonly PostRepository _postRepository;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly CommentRepository _commentRepository;
        private readonly GolferRepository _golferRepository;
        private readonly UserManager<Golfer> _golferManager;
        private readonly CourseRepository _courseRepository;
        private readonly TournamentMemberRepository _tournamentMemberRepository;
        private readonly TournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<HubSignalR> _hubContext;

        public NotificationService(
            UserManager<Golfer> golferManager, 
            TournamentMemberRepository tournamentMemberRepository, 
            TournamentRepository tournamentRepository, 
            GroupMemberRepository groupMemberRepository, 
            CourseRepository courseRepository, 
            PostRepository postRepository, 
            GroupRepository groupRepository, 
            CommentRepository commentRepository, 
            IHubContext<HubSignalR> hubContext, 
            IMapper mapper, 
            NotificationRepository notificationRepository, 
            GolferRepository golferRepository)
        {
            _golferManager = golferManager;
            _tournamentMemberRepository = tournamentMemberRepository;
            _tournamentRepository = tournamentRepository;
            _groupMemberRepository = groupMemberRepository;
            _notificationRepository = notificationRepository;
            _golferRepository = golferRepository;
            _mapper = mapper;
            _hubContext = hubContext;
            _commentRepository = commentRepository;
            _groupRepository = groupRepository;
            _postRepository = postRepository;
            _courseRepository = courseRepository;
        }

        public async Task<NotificationResponse> Add(NotificationRequest notificationRequest)
        {
            NotificationResponse response = new NotificationResponse();
            var golfer = _golferRepository.Get(notificationRequest.golferID);
            if (golfer == null)
            {
                throw new NotFoundException("Can't find Golfer");
            }
            Notification notification = new Notification();
            notification.golferID = notificationRequest.golferID;
            notification.IsViewed = false;
            notification.Objects = notificationRequest.Objects;
            notification.Type = notificationRequest.Type;
            notification.ReferObject = notificationRequest.ReferObject;
            notification.Content = notificationRequest.Content;
            _notificationRepository.Add(notification);
            foreach (var j in notification.Objects)
            {
                if (j.Type == (int)ObjectType.User)
                {
                    response.Image = j.Image;
                    break;
                }
            }
            //Send Notification to cient 
            //_hubContext.Clients.Client(HubSignalR.GetConnectId(notificationRequest.golferID)).SendAsync("receiveNotification", notification.Objects, notification.Content, notification.ReferObject, response.Image, notification.Type);
            await _hubContext.Clients.Group(notification.golferID.ToString()).SendAsync("receiveNotification", notification.Objects, notification.Content, notification.ReferObject, response.Image, notification.Type);
            var result = _notificationRepository.Get(notification.ID);
            _mapper.Map(result, response);

            return response;
        } 
        public async Task<bool> UpDate(Notification notification)
        {

            NotificationResponse response = new NotificationResponse();
            _notificationRepository.UpdateEntity(notification);
            foreach (var j in notification.Objects)
            {
                if (j.Type == (int)ObjectType.User)
                {
                    response.Image = j.Image;
                    break;
                }
            }
            //Send Notification to cient 
            //_hubContext.Clients.Client(HubSignalR.GetConnectId(notificationRequest.golferID)).SendAsync("receiveNotification", notification.Objects, notification.Content, notification.ReferObject, response.Image, notification.Type);
            await _hubContext.Clients.Group(notification.golferID.ToString()).SendAsync("receiveNotification", notification.Objects, notification.Content, notification.ReferObject, response.Image, notification.Type);
            return true;
        }
        public bool View(Guid golferID, Guid notificationID)
        {

            var notification = _notificationRepository.Get(notificationID);
            if (notification == null)
                return false;
            if (notification.IsViewed == false && notification.golferID == golferID)
            {
                notification.IsViewed = true;
                _notificationRepository.UpdateEntity(notification);
                return true;
            }

            return false;

        }
        public bool ViewAll(Guid golferID)
        {
            var notifications = _notificationRepository.Find(n => n.IsViewed == false && n.golferID == golferID);
            if (notifications.Count() > 0)
            {
                foreach (var i in notifications.ToList())
                {
                    if (i.IsViewed == false)
                    {
                        i.IsViewed = true;
                        _notificationRepository.UpdateEntity(i);

                    }
                }
                return true;
            }
            else
                return false;

        }
        public bool Delete(Guid golferID, Guid notificationID)
        {

            var notification = _notificationRepository.Get(notificationID);
            if (notification.golferID == golferID && notification.DeletedDate == null)
            {
                notification.DeletedDate = DateTime.Now;
                _notificationRepository.UpdateEntity(notification);
                return true;
            }
            return false;

        }
        public int UnReadNotification(Guid golferID)
        {
            return _notificationRepository.CountAll(n => n.golferID == golferID && n.IsViewed == false);
        }

        public List<NotificationResponse> GetMyNotifiction(Guid golferID, int startIndex)
        {
            List<NotificationResponse> responses = new List<NotificationResponse>();
            var golfer = _golferRepository.Get(golferID);
            if (golfer == null)
            {
                throw new NotFoundException("This golfer don't exit!");
            }
            var result = _notificationRepository.Find(nt => nt.golferID == golfer.Id && nt.DeletedDate == null).OrderBy(nt => nt.IsViewed).OrderByDescending(nt => nt.CreatedDate).Skip(startIndex).Take(Const.PageSize);
            if (result.Count() == 0)
                return new List<NotificationResponse>();
            else
                foreach (var i in result)
                {

                    this.View(golferID, i.ID);
                    NotificationResponse response = new NotificationResponse();

                    _mapper.Map(i, response);
                    responses.Add(response);
                    if (i.Objects.Count() == 0)
                        continue;
                    foreach (var j in i.Objects)
                    {
                        if (j.Type == ObjectType.User)
                        {
                            response.Image = _golferRepository.Get(j.ID).Avatar;
                            break;
                        }

                    }

                }
            return responses;
        }

        public NotificationObject getNotificationObject(Guid ID, int type)
        {
            NotificationObject notificationObject = new NotificationObject();
            notificationObject.Type = -1;
            switch (type)
            {
                case ObjectType.Comment:
                    {
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = "comment";
                        return notificationObject;
                    }
                case ObjectType.Group:
                    {
                        var group = _groupRepository.Get(ID);
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = group.Name;
                        notificationObject.Image = group.Cover;
                        return notificationObject;
                    }
                case ObjectType.GroupPost:
                    {
                        //var post = _postRepository.Get(ID);
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = "post";
                        return notificationObject;
                    }
                case ObjectType.Post:
                    {
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = "post";
                        return notificationObject;
                    }
                case ObjectType.User:
                    {
                        var user = _golferRepository.Get(ID);
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = user.FirstName + " " + user.LastName;
                        notificationObject.Image = user.Avatar;
                        return notificationObject;
                    }
                case ObjectType.Course:
                    {
                        var course = _courseRepository.Get(ID);
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = course.Name;
                        notificationObject.Image = course.Cover;
                        return notificationObject;
                    }
                case ObjectType.Tournament:
                    {
                        var tournament = _tournamentRepository.Get(ID);
                        notificationObject.ID = ID;
                        notificationObject.Type = type;
                        notificationObject.Name = tournament.TournamentName;
                        return notificationObject;
                    }

                default: return notificationObject;
            }
        }
        //relationship
        public void NotificationAddFriendRequest(Guid currentGolferID, Guid GolferID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.AddFriend;
            notificationRequest.ReferObject = this.getNotificationObject(currentGolferID, ObjectType.User);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.SendAddFriendRequest+".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationConfirmAddFriendRequest(Guid currentGolferID, Guid GolferID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.AddFriend;
            notificationRequest.ReferObject = this.getNotificationObject(currentGolferID, ObjectType.User);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.ConfirmAddFriendRequest + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        //post notification
        public void NotificationTagUserInPost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.PostTag;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.TagUserInPost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationCommentYousTagPost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Comment;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.CommentYousTagPost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationLikeYourComment(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Like;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.LikeYourComment + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationCommentYousPost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {

            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Comment;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.CommentYousPost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationRepliedYourComment(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Comment;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.RepliedYourComment + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationLikeYourPost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Like;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.LikeYourPost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationLikeYourTagPost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Like;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.LikeYourTagPost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationMentionsUserComment(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.CommentTag;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.MentionsUserComment + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationPlayWithFriend(Guid currentGolferID, Guid GolferID, Guid PostID, Guid CourseID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Checkin;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = NotificationContent.PlayWith + " " + "@" + currentGolferID + " " + NotificationContent.InCourse + " " + this.getNotificationObject(CourseID, ObjectType.Course) + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationEnterYourScorecard(Guid currentGolferID, Guid GolferID, Guid PostID)
        {

            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.EnterScorecarrds;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.EnterYourScorecard + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationConfirmScorecard(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.ConfirmScorecarrds;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.ConfirmScorecard + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationConfirmIncorrectScorecard(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.ConfirmScorecarrds;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.ConfirmIncorrectScorecard + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationSharePost(Guid currentGolferID, Guid GolferID, Guid PostID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Share;
            notificationRequest.ReferObject = this.getNotificationObject(PostID, ObjectType.Post);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.SharePost + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        //group notification
        public void NotificationInviteGroupMember(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.AddGroupMember + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationAddGroupMember(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.AddGroupMember + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationRemoveGroup( Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Content ="The " +"@"+notificationRequest.ReferObject.ID+" "+ NotificationContent.DeleteGroup + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public async Task NotificationSendGroupMemberRequest(Guid currentGolferID, Guid GroupID)
        {
            var admins = _groupMemberRepository.Find(gm => gm.GroupID == GroupID && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Admin)).ToList();
            var countRequest = _groupMemberRepository.CountAll(gm => gm.Status == MemberStatus.Request && gm.GroupID == GroupID);
            foreach (var i in admins)
            {
                var notifi = _notificationRepository.Find(n => n.golferID == i.GolferID && n.Type == NotificationType.Group &&n.ReferObject.ID==GroupID && n.Content.Contains(NotificationContent.SendAddFriendRequest)).FirstOrDefault();
                if (notifi == null)
                {
                    NotificationRequest notificationRequest = new NotificationRequest();
                    notificationRequest.golferID = i.GolferID;
                    notificationRequest.Type = NotificationType.Group;
                    notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
                    notificationRequest.Objects.Add(notificationRequest.ReferObject);
                    notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
                    notificationRequest.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest - 1) + " others") + " " + NotificationContent.SendGroupMemberRequest + " @" + notificationRequest.ReferObject.ID + ".";
                    await this.Add(notificationRequest);
                }
                else
                {
                    notifi.IsViewed = false;
                    notifi.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest - 1) + " others") + " " + NotificationContent.SendGroupMemberRequest + " @" + notifi.ReferObject.ID + ".";
                    notifi.CreatedDate = DateTime.Now;
                    var tmp = this.UpDate(notifi).Result;
                }
            }

        }
        public void NotificationRemoveGroupMember(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.RemoveGroupMember + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationConfirmGroupMember(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.ConfirmGroupMember + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public void NotificationRejectModerator(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.RejectModerator + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationGroupMemberPostNews(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.GroupMemberPostNews + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }

        public void NotificationAddModerator(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.AddGroupModerator + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public async Task NotificationAddAdmin(Guid currentGolferID, Guid GolferID, Guid GroupID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Group;
            notificationRequest.ReferObject = this.getNotificationObject(GroupID, ObjectType.Group);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.AddGroupAdmin + " @" + notificationRequest.ReferObject.ID + ".";
            await this.Add(notificationRequest);
        }
        //event notification
        public async Task NotificationInviteFriendJoinEvent(Guid currentGolferID, Guid GolferID, Event ev)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Event;
            notificationRequest.ReferObject = new NotificationObject() { ID = ev.ID, Name = ev.Course.Name, Type = ObjectType.Event };
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID.ToString() + " " + NotificationContent.InviteFriend + " at " + ev.Time.ToString("HH:mm") + " on " + ev.Time.ToString("dd/MM/yyyy") + " at @" + notificationRequest.ReferObject.ID.ToString() + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        //scorecard notification
        public void NotificationInviteFriendConfirmScorecard(Guid currentGolferID, Guid GolferID, Guid postID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.ConfirmScorecarrds;
            notificationRequest.ReferObject = new NotificationObject() { ID = postID, Type = ObjectType.Post };
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID.ToString() + " " + NotificationContent.InviteFriendConfirmscorecard + ".";
            var tmp = this.Add(notificationRequest).Result;
        }
        public async Task NotificationReportScorecard(Guid currentGolferID, Guid GolferID, Guid scorecardID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.ConfirmScorecarrds;
            notificationRequest.ReferObject = new NotificationObject() { ID = scorecardID, Type = ObjectType.Scorecard };
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID.ToString() + " " + NotificationContent.ReportScorecard + ".";
            await this.Add(notificationRequest);
        }
        //tournament notification
        /// <summary>
        /// thông báo gửi tới admin giải đấu khi có yêu cầu 
        /// </summary>
        /// <param name="currentGolferID">Người gửi yêu cầu tham gia giải đấu</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        public void NotificationSendTournamentMemberRequest(Guid currentGolferID, Guid tournamentID)
        {
            var countRequest = _tournamentMemberRepository.CountAll(tm=>tm.TuornamentID==tournamentID&&tm.MemberStatus==TournamentMemberStatus.Requested);
            var adminID = _tournamentRepository.Get(tournamentID).OwnerID;
            var notifi = _notificationRepository.Find(n => n.golferID == adminID && n.Type == NotificationType.Tournament && n.ReferObject.ID==tournamentID&& n.Content.Contains(NotificationContent.SendTournamentMemberRequest)).FirstOrDefault();
            if (notifi == null)
            {
                NotificationRequest notificationRequest = new NotificationRequest();
                notificationRequest.golferID = adminID;
                notificationRequest.Type = NotificationType.Tournament;
                notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
                notificationRequest.Objects.Add(notificationRequest.ReferObject);
                notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
                notificationRequest.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest-1) + " others") + " " + NotificationContent.SendTournamentMemberRequest + " @" + notificationRequest.ReferObject.ID + ".";
                var tmp= this.Add(notificationRequest).Result;
            }
            else
            {
                notifi.IsViewed = false;
                notifi.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest - 1) + " others") + " " + NotificationContent.SendTournamentMemberRequest + " @" + notifi.ReferObject.ID + ".";
                notifi.CreatedDate = DateTime.Now;
                var tmp = this.UpDate(notifi).Result;
            }
        }
        /// <summary>
        /// Gửi yêu cầu tạo nhóm
        /// </summary>
        /// <param name="currentGolferID"></param>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        public async Task NotificationSendTournamentRequest(Guid currentGolferID, Guid tournamentID)
        {
            var countRequest = _tournamentRepository.CountAll(tm=>tm.Status==TournamentStatus.Requested);
            var admin = await _golferManager.GetUsersInRoleAsync(RoleNormalizedName.SystemAdmin);
            var adminIDs = admin.Select(a=>a.Id);
            foreach(var i in adminIDs)
            {
                var notifi = _notificationRepository.Find(n => n.golferID == i && n.Type == NotificationType.Tournament &&n.ReferObject.ID==tournamentID && n.Content.Contains(NotificationContent.SendTournamentRequest)).FirstOrDefault();
                if (notifi == null)
                {
                    NotificationRequest notificationRequest = new NotificationRequest();
                    notificationRequest.golferID = i;
                    notificationRequest.Type = NotificationType.Tournament;
                    notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
                    notificationRequest.Objects.Add(notificationRequest.ReferObject);
                    notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
                    notificationRequest.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest - 1) + " others") + " " + NotificationContent.SendTournamentRequest + " @" + notificationRequest.ReferObject.ID + ".";
                    await this.Add(notificationRequest);
                }
                else
                {
                    notifi.IsViewed = false;
                    notifi.Content = "@" + currentGolferID + " " + ((countRequest - 1) == 0 ? "" : "and " + (countRequest - 1) + " others") + " " + NotificationContent.SendTournamentRequest + " @" + notifi.ReferObject.ID + ".";
                    notifi.CreatedDate = DateTime.Now;
                    var tmp = this.UpDate(notifi).Result;
                }
            }

        }
        /// <summary>
        /// Xác nhận yều cầu tham gia nhóm
        /// </summary>
        /// <param name="currentGolferID">định danh admin</param>
        /// <param name="GolferID">Định danh người gửi yêu cầu</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        public void NotificationConfirmTournamentMember(Guid currentGolferID, Guid GolferID, Guid tournamentID)
        {

            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Tournament;
            notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.ConfirmTournamentMember + " @" + notificationRequest.ReferObject.ID + ".";
            var tmp= this.Add(notificationRequest).Result;
        }
        public async Task NotificationConfirmTournament(Guid currentGolferID, Guid GolferID, Guid tournamentID)
        {

            var admin = await _golferManager.GetUsersInRoleAsync(RoleNormalizedName.SystemAdmin);
            var adminIDs = admin.Select(a => a.Id);
            if(adminIDs.Contains(currentGolferID))
            {

                NotificationRequest notificationRequest = new NotificationRequest();
                notificationRequest.golferID = GolferID;
                notificationRequest.Type = NotificationType.Tournament;
                notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
                notificationRequest.Objects.Add(notificationRequest.ReferObject);
                //notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
                notificationRequest.Content = NotificationContent.ConfirmTournamentFirst + " @ " + notificationRequest.ReferObject.ID +" "+ NotificationContent.ConfirmTournamentLast + ".";
                await this.Add(notificationRequest);
            }    

        }
        public async Task NotificationRejectTournament(Guid currentGolferID, Guid GolferID, Guid tournamentID)
        {

            var admin = await _golferManager.GetUsersInRoleAsync(RoleNormalizedName.SystemAdmin);
            var adminIDs = admin.Select(a => a.Id);
            if(adminIDs.Contains(currentGolferID))
            {

                NotificationRequest notificationRequest = new NotificationRequest();
                notificationRequest.golferID = GolferID;
                notificationRequest.Type = NotificationType.Tournament;
                notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
                notificationRequest.Objects.Add(notificationRequest.ReferObject);
                notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
                notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.RejectTournament + " @" + notificationRequest.ReferObject.ID + ".";
                await this.Add(notificationRequest);
            }    

        }
        public void NotificationRejectTournamentMember(Guid currentGolferID, Guid GolferID, Guid tournamentID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Tournament;
            notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Objects.Add(this.getNotificationObject(currentGolferID, ObjectType.User));
            notificationRequest.Content = "@" + currentGolferID + " " + NotificationContent.RejectTournamentMember + " @" + notificationRequest.ReferObject.ID + ".";
             var tmp = this.Add(notificationRequest).Result;
        } 
        public void NotificationDeleteTournament( Guid GolferID, Guid tournamentID)
        {
            NotificationRequest notificationRequest = new NotificationRequest();
            notificationRequest.golferID = GolferID;
            notificationRequest.Type = NotificationType.Tournament;
            notificationRequest.ReferObject = this.getNotificationObject(tournamentID, ObjectType.Tournament);
            notificationRequest.Objects.Add(notificationRequest.ReferObject);
            notificationRequest.Content = "The " + "@" + notificationRequest.ReferObject.ID + " " + NotificationContent.DeleteTournament + ".";
            var tmp= this.Add(notificationRequest).Result;
        }

        public bool CheckNotificationSetting(int notificationType, NotificationSetting notificationSetting)
        {
            if (notificationSetting.Comment == false)
            {
                if (notificationType == NotificationType.Comment || notificationType == NotificationType.CommentTag)
                {
                    return false;
                }
            }
            if (notificationSetting.MakeFriend == false)
            {
                if (notificationType == NotificationType.AddFriend)
                {
                    return false;
                }
            }
            if (notificationSetting.Tag == false)
            {
                if (notificationType == NotificationType.CommentTag || notificationType == NotificationType.PostTag)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
