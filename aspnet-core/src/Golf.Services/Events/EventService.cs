using AutoMapper;
using Golf.Core.Dtos.Controllers.EventController.Request;
using Golf.Core.Dtos.Controllers.EventController.Response;
using Golf.Core.Exceptions;
using Golf.Domain.Events;
using Golf.Domain.Messages;
using Golf.Domain.Post;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Event;
using Golf.Domain.Shared.Post;
using Golf.Domain.Shared.Relationship;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Events
{
    public class EventService
    {
        private readonly EventRepository _eventRepository;
        private readonly GolferService _golferService;
        private readonly RelationshipRepository _relationshipRepository;
        private readonly NotificationService _notificationService;
        private readonly CourseRepository _courseRepository;
        private readonly PostRepository _postRepository;
        private readonly ConversationRepository _conversationRepository;
        private readonly IMapper _mapper;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly RelationshipService _relationshipService;
        public EventService(RelationshipService relationshipService, DatabaseTransaction databaseTransaction, PostRepository postRepository, ConversationRepository conversationRepository, RelationshipRepository relationshipRepository, CourseRepository courseRepository, EventRepository eventRepository, GolferService golferService, IMapper mapper, NotificationService notificationService)
        {
            _relationshipService = relationshipService;
            _databaseTransaction = databaseTransaction;
            _postRepository = postRepository;
            _eventRepository = eventRepository;
            _golferService = golferService;
            _notificationService = notificationService;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _relationshipRepository = relationshipRepository;
            _conversationRepository = conversationRepository;
        }

        /// <summary>
        /// Tạo sự kiện
        /// </summary>
        /// <param name="curentGolferID">Định danh người dùng hiện thời</param>
        /// <param name="addEventRequest">Dự liệu tạo sự kiện</param>
        /// <returns>Dữ liệu sự kiện đã tạo</returns>
        public async Task<EventResponse> Add(Guid curentGolferID, AddEventRequest addEventRequest)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                if (_courseRepository.Get(addEventRequest.CourseID) == null)
                    throw new NotFoundException("Not found coure!");
                if (addEventRequest.MaxMembers > 4)
                    throw new BadRequestException("Max member less than 4");
                Event ev = new Event();
                _mapper.Map(addEventRequest, ev);
                ev.AddMemberID(curentGolferID);
                ev.OwnerID = curentGolferID;
                _eventRepository.SafeAdd(ev);
                //this.AddConversation(curentGolferID, ev.ID);
                if (ev.InviteFriend == true)
                {
                    var friends = _relationshipRepository.Find(r => (r.FriendID == curentGolferID || r.GolferID == curentGolferID) && r.Status == RelationshipStatus.IsFriend).ToList();
                    foreach (var i in friends)
                    {
                        if (curentGolferID == i.FriendID)
                        {
                            await _notificationService.NotificationInviteFriendJoinEvent(curentGolferID, i.GolferID, ev);
                        }
                        else
                        {
                            await _notificationService.NotificationInviteFriendJoinEvent(curentGolferID, i.FriendID, ev);

                        }
                    }
                }
                if (ev.PostNewfeed == true)
                {
                    var post = new Post();
                    //post.Type = PostType.Event;
                    post.OwnerID = curentGolferID;
                    //post.PostFeeling = PostFeeling.ShareEvent;
                    post.SetEventID(ev.ID);
                    _postRepository.SafeAdd(post);
                }
                await _databaseTransaction.Commit();
                return this.GetEventDetail(curentGolferID, ev);
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Sử thông tin sự kiện
        /// </summary>
        /// <param name="curentGolferID">định danh người dùng</param>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <param name="addEventRequest">Dữ liệu cập nhật sự kiện</param>
        /// <returns>Dư liệu sự kiện đă cập nhật</returns>
        async public Task<EventResponse> Edit(Guid curentGolferID, Guid eventID, AddEventRequest addEventRequest)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                if (_courseRepository.Get(addEventRequest.CourseID) == null)
                    throw new NotFoundException("Not found coure!");
                Event ev = _eventRepository.Get(eventID);
                if (ev == null)
                    throw new NotFoundException("Not found event!");
                if (curentGolferID != ev.OwnerID)
                {
                    throw new ForbiddenException("Can not edit this event");

                }
                if (ev.PostNewfeed == true)
                {
                    addEventRequest.PostNewfeed = true;
                }
                else
                {
                    if (addEventRequest.PostNewfeed == true)
                    {
                        var post = new Post();
                        //post.Type = PostType.Event;
                        post.OwnerID = curentGolferID;
                        //post.PostFeeling = PostFeeling.ShareEvent;
                        post.SetEventID(ev.ID);
                        _postRepository.SafeAdd(post);
                    }
                }
                if (ev.InviteFriend == true)
                {
                    addEventRequest.InviteFriend = true;
                }
                else
                {
                    if (addEventRequest.InviteFriend == true)
                    {
                        var friends = _relationshipRepository.Find(r => (r.FriendID == curentGolferID || r.GolferID == curentGolferID) && r.Status == RelationshipStatus.IsFriend);
                        foreach (var i in friends)
                        {
                            if (curentGolferID == i.FriendID)
                            {
                                await _notificationService.NotificationInviteFriendJoinEvent(curentGolferID, i.GolferID, ev);
                            }
                            else
                            {
                                await _notificationService.NotificationInviteFriendJoinEvent(curentGolferID, i.FriendID, ev);
                            }
                        }
                    }
                }
                _mapper.Map(addEventRequest, ev);
                _eventRepository.SafeUpdate(ev);
                await _databaseTransaction.Commit();
                return this.GetEventDetail(curentGolferID, ev);
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// Xóa sự kiện
        /// </summary>
        /// <param name="curentGolferID"></param>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public bool Delete(Guid curentGolferID, Guid eventID)
        {
            Event ev = _eventRepository.Get(eventID);
            if (ev == null)
                throw new NotFoundException("Not found event!");
            if (curentGolferID != ev.OwnerID)
            {
                throw new ForbiddenException("Can not edit this event");

            }
            if (ev.ConvesationID != null)
            {
                var conversation = _conversationRepository.Get((Guid)ev.ConvesationID);
                if (conversation != null)
                {
                    _conversationRepository.RemoveEntity(conversation);
                }
            }
            _eventRepository.RemoveEntity(ev);
            return true;
        }

        /// <summary>
        /// Tham gia sự kiện
        /// </summary>
        /// <param name="currentID">định danh ngươi dùng</param>
        /// <param name="eventID">Định danh sự kiện</param>
        /// <returns>Kết quả</returns>
        public bool JoinEvent(Guid currentID, Guid eventID)
        {
            Event ev = _eventRepository.Get(eventID);
            if (ev == null)
                throw new NotFoundException("Not found event!");
            if (ev.Privacy == EventPrivacy.Friends && _relationshipService.IsFriend(currentID, ev.OwnerID) == false)
            {
                throw new BadRequestException("Not allow!");
            }
            if (ev.GetMemberIDs().FindAll(i => i == currentID).Count() > 0 || ev.GetMemberIDs().Count() >= ev.MaxMembers)
            {
                throw new BadRequestException("This user was joined event");
            }
            ev.AddMemberID(currentID);
            _eventRepository.UpdateEntity(ev);
            if (ev.ConvesationID != null)
            {
                var conversation = _conversationRepository.Get((Guid)ev.ConvesationID);
                conversation.AddGolferID(currentID);
                _conversationRepository.UpdateEntity(conversation);
            }
            return true;
        }

        /// <summary>
        /// Rời sự kiện 
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="eventID">Dịnh danh dự kiện</param>
        /// <returns></returns>
        public bool ExitEvent(Guid currentID, Guid eventID)
        {
            Event ev = _eventRepository.Get(eventID);
            if (ev == null)
                throw new NotFoundException("Not found event!");
            if (ev.GetMemberIDs().Find(i => i == currentID) == null)
            {
                throw new BadRequestException("This user wasn't joined event");
            }
            if (ev.OwnerID == currentID)
            {
                if (ev.ConvesationID != null)
                {
                    _conversationRepository.RemoveEntity(_conversationRepository.Get((Guid)ev.ConvesationID));
                }
                _eventRepository.RemoveEntity(ev);


                return true;
            }
            ev.RemoveMemberID(currentID);
            _eventRepository.UpdateEntity(ev);
            if (ev.ConvesationID != null)
            {
                var conversation = _conversationRepository.Get((Guid)ev.ConvesationID);
                conversation.RemoveGolferID(currentID);
                _conversationRepository.UpdateEntity(conversation);
            }
            return true;
        }
        /// <summary>
        /// Lấy thông tin sự kiện từ định danh
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public EventResponse Get(Guid currentID, Guid eventID)
        {
            List<Event> evs = _eventRepository.Find(e => e.ID == eventID).ToList();
            if (evs.Count() <= 0)
                throw new NotFoundException("Not found event!");
            if (evs.First().Privacy == EventPrivacy.Friends && _relationshipService.IsFriend(currentID, evs.First().OwnerID) == false)
            {
                throw new BadRequestException("Not allow!");
            }
            return this.GetEventDetail(currentID, evs.First());
        }
        public List<EventResponse> FilterEvent(Guid currentID, EventFilterByStatus? eventFilter, EventPrivacy? eventPrivacy, DateTime? date, string searchKey, int startIndex)
        {
            var relationship = _relationshipRepository.Find(e => e.Status == RelationshipStatus.IsFriend && (e.GolferID == currentID || e.FriendID == currentID));
            List<Guid> friendID = new List<Guid>();
            if (relationship.Count() > 0)
            {
                foreach (var i in relationship)
                {
                    if (currentID == i.FriendID)
                    {
                        friendID.Add(i.GolferID);
                    }
                    if (currentID == i.GolferID)
                    {
                        friendID.Add(i.FriendID);
                    }
                }
            }
            List<Event> events = new List<Event>();
            switch (eventFilter)
            {
                case EventFilterByStatus.All:
                    {
                        events = _eventRepository.Find(e => (
                        eventPrivacy == null ? (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID)) : (e.Privacy == (EventPrivacy)eventPrivacy && ((EventPrivacy)eventPrivacy == EventPrivacy.Friends ? friendID.Contains(e.OwnerID) : true)))
                        && (date == null ? true : DateTime.Compare(e.Time.Date, ((DateTime)date).Date) == 0)
                        && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower())
                        && DateTime.Compare(DateTime.Now, e.Time) < 0
                        && (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID))).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByStatus.Registered:
                    {
                        events = _eventRepository.Find(e =>
                        (eventPrivacy == null ? (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID)) : (e.Privacy == (EventPrivacy)eventPrivacy && ((EventPrivacy)eventPrivacy == EventPrivacy.Friends ? friendID.Contains(e.OwnerID) : true)))
                        && (date == null ? true : DateTime.Compare(e.Time.Date, ((DateTime)date).Date) == 0)
                        && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower())
                        && e.MemberIDs.Contains(currentID.ToString()) && DateTime.Compare(DateTime.Now, e.Time) < 0).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByStatus.Completed:
                    {
                        events = _eventRepository.Find(e =>
                        (eventPrivacy == null ? (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID)) : (e.Privacy == (EventPrivacy)eventPrivacy && ((EventPrivacy)eventPrivacy == EventPrivacy.Friends ? friendID.Contains(e.OwnerID) : true)))
                        && (date !!= null ? true : DateTime.Compare(e.Time.Date, ((DateTime)date).Date) == 0)
                        && (e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower()))
                        && e.MemberIDs.Contains(currentID.ToString())
                        && DateTime.Compare(DateTime.Now, e.Time) > 0).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                default:
                    {
                        events = _eventRepository.Find(e =>
                        (eventPrivacy == null ? (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID)) : (e.Privacy == (EventPrivacy)eventPrivacy && ((EventPrivacy)eventPrivacy == EventPrivacy.Friends ? friendID.Contains(e.OwnerID) : true)))
                        && (date == null ? true : DateTime.Compare(e.Time.Date, ((DateTime)date).Date) == 0)
                        && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower())
                        && (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID))).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
            }
            List<EventResponse> responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(this.GetEventDetail(currentID, ev));
            }
            return responses;
        }
        /// <summary>
        /// Lọc sự kiện theo tiêu chí
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="eventFilter"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<EventResponse> GetEventByFilter(Guid currentID, EventFilterByStatus eventFilter, int startIndex)
        {
            List<Event> events = new List<Event>();
            var relationship = _relationshipRepository.Find(e => e.Status == RelationshipStatus.IsFriend && (e.GolferID == currentID || e.FriendID == currentID));
            List<Guid> friendID = new List<Guid>();
            if (relationship.Count() > 0)
            {
                foreach (var i in relationship)
                {
                    if (currentID == i.FriendID)
                    {
                        friendID.Add(i.GolferID);
                    }
                    if (currentID == i.GolferID)
                    {
                        friendID.Add(i.FriendID);
                    }
                }
            }
            switch (eventFilter)
            {
                case EventFilterByStatus.All:
                    {
                        events = _eventRepository.Find(e => DateTime.Compare(DateTime.Now, e.Time) < 0 && (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID))).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByStatus.Registered:
                    {
                        events = _eventRepository.Find(e => e.MemberIDs.Contains(currentID.ToString()) && DateTime.Compare(DateTime.Now, e.Time) < 0).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByStatus.Completed:
                    {
                        events = _eventRepository.Find(e => e.MemberIDs.Contains(currentID.ToString()) && DateTime.Compare(DateTime.Now, e.Time) > 0).OrderBy(e => e.Time).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                default:
                    throw new BadRequestException("Not valid filter");
            }
            List<EventResponse> responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(this.GetEventDetail(currentID, ev));
            }
            return responses;
        }
        /// <summary>
        /// Tìm kiếm sự kiệ theo tên sân đấu
        /// </summary>
        /// <param name="currentID">định danh ngươi dùng</param>
        /// <param name="searchKey">Từ khóa tên sân</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        public List<EventResponse> SearchEvent(Guid currentID, EventFilterByPrivacy eventFilterByPrivacy, string searchKey, int startIndex)
        {
            var relationship = _relationshipRepository.Find(e => e.Status == RelationshipStatus.IsFriend && (e.GolferID == currentID || e.FriendID == currentID));
            List<Guid> friendID = new List<Guid>();
            if (relationship.Count() > 0)
            {
                foreach (var i in relationship)
                {
                    if (currentID == i.FriendID)
                    {
                        friendID.Add(i.GolferID);
                    }
                    if (currentID == i.GolferID)
                    {
                        friendID.Add(i.FriendID);
                    }
                }
            }
            List<Event> events = new List<Event>();
            switch (eventFilterByPrivacy)
            {
                case EventFilterByPrivacy.All:
                    {
                        events = _eventRepository.Find(e => DateTime.Compare(DateTime.Now, e.Time) < 0 && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower()) && (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID))).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByPrivacy.Public:
                    {
                        events = _eventRepository.Find(e => DateTime.Compare(DateTime.Now, e.Time) < 0 && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower()) && (e.Privacy == EventPrivacy.Public)).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case EventFilterByPrivacy.Friends:
                    {
                        events = _eventRepository.Find(e => DateTime.Compare(DateTime.Now, e.Time) < 0 && e.Course.Name.Trim().ToLower().Contains(searchKey.Trim().ToLower()) && (e.Privacy == EventPrivacy.Friends && friendID.Contains(e.OwnerID))).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
            }
            // var tmp = _eventRepository.Find(e => e.GetMemberIDs().FindAll(m => m == currentID).Count() > 0);//.Contains());
            List<EventResponse> responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(this.GetEventDetail(currentID, ev));
            }
            return responses;
        }
        /// <summary>
        /// lọc sự kiệ theo tiêu chí thời gian
        /// </summary>
        /// <param name="currentID">định danh người dùng</param>
        /// <param name="date">Bộ lọc thời gian</param>
        /// <param name="startIndex">VVij trí phân trang</param>
        /// <returns></returns>
        public List<EventResponse> FilterEventByDate(Guid currentID, DateTime date, int startIndex)
        {
            var relationship = _relationshipRepository.Find(e => e.Status == RelationshipStatus.IsFriend && (e.GolferID == currentID || e.FriendID == currentID));
            List<Guid> friendID = new List<Guid>();
            if (relationship.Count() > 0)
            {
                foreach (var i in relationship)
                {
                    if (currentID == i.FriendID)
                    {
                        friendID.Add(i.GolferID);
                    }
                    if (currentID == i.GolferID)
                    {
                        friendID.Add(i.FriendID);
                    }
                }
            }
            var events = _eventRepository.Find(e => DateTime.Compare(e.Time.Date, date.Date) == 0 && (e.Privacy == EventPrivacy.Public || friendID.Contains(e.OwnerID))).Skip(startIndex).Take(Const.PageSize).ToList();
            // var tmp = _eventRepository.Find(e => e.GetMemberIDs().FindAll(m => m == currentID).Count() > 0);//.Contains());
            List<EventResponse> responses = new List<EventResponse>();
            foreach (var ev in events)
            {

                responses.Add(this.GetEventDetail(currentID, ev));
            }
            return responses;
        }
        /// <summary>
        /// Tạo cuộc trò chuyện cho sự kiện
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="eventID"></param>
        /// <returns></returns>
        public Guid AddConversation(Guid currentID, Guid eventID, string conversationName)
        {
            Event ev = _eventRepository.Get(eventID);
            if (ev == null)
                throw new NotFoundException("Not found event!");
            if (ev.ConvesationID != null)
                throw new BadRequestException("Conversation is exit!");
            if (ev.OwnerID != currentID)
                throw new BadRequestException("Not allow!");
            Conversation conversation = new Conversation();
            conversation.Name = (conversationName == null || conversationName == "") ? "Lịch chơi " + ev.Time.ToString("HH:mm") + " ngày " + ev.Time.ToString("dd/MM") : conversationName;
            conversation.GolferIDs = currentID.ToString();
            foreach (var i in ev.GetMemberIDs())
            {
                if (i != currentID)
                {
                    conversation.AddGolferID(i);
                }
            }
            _conversationRepository.Add(conversation);
            ev.ConvesationID = conversation.ID;
            _eventRepository.UpdateEntity(ev);
            return conversation.ID;
        }

        public EventResponse GetEventDetail(Guid currentID, Event ev)
        {
            EventResponse eventResponse = new EventResponse();
            _mapper.Map(ev, eventResponse);
            eventResponse.Address = _courseRepository.Find(c => c.ID == ev.CourseID).FirstOrDefault().Location.Address;
            eventResponse.Members = _golferService.GetMinimizedGolfers(ev.GetMemberIDs());
            eventResponse.Joined = (ev.GetMemberIDs().FindAll(e => e == currentID).Count() > 0) ? true : false;
            if (ev.Privacy == EventPrivacy.Friends && _relationshipService.IsFriend(currentID, ev.OwnerID) == false)
            {
                return null;
                //EventResponse e = new EventResponse();
                //e.AllowViewing = _relationshipService.IsFriend(currentID, ev.OwnerID);
                //return e;
            }
            return eventResponse;
        }
    }
}
