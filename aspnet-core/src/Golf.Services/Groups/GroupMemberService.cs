using Golf.Domain.Shared;
using Golf.Core.Exceptions;
using Golf.Domain.SocialNetwork;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Golf.Domain.Shared.Resources;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.GroupController.Response;
using Golf.Domain.GolferData;
using Golf.Services.Notifications;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using AutoMapper;
using Golf.Domain.Shared.Relationship;
using Golf.EntityFrameworkCore;
using Golf.Domain.Shared.Groups;

namespace Golf.Services
{
    public class GroupMemberService
    {
        readonly private GroupMemberRepository _groupMemberRepository;
        readonly private GroupRepository _groupRepository;
        private readonly GolferRepository _golferRepository;
        private readonly GolferService _golferService;
        private readonly StatisticService _statisticService;
        private readonly NotificationService _notificationService;
        private readonly RelationshipRepository _relationshipRepository;
        private readonly RelationshipService _relationshipService;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly IMapper _mapper;
        public GroupMemberService(GolferService golferService, DatabaseTransaction databaseTransaction, RelationshipService relationshipService, RelationshipRepository relationshipRepository, IMapper mapper, NotificationService notificationService, StatisticService statisticService, GroupMemberRepository groupMemberRepository, GroupRepository groupRepository, GolferRepository golferRepository)
        {
            _golferService = golferService;
            _databaseTransaction = databaseTransaction;
            _relationshipService = relationshipService;
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;
            _golferRepository = golferRepository;
            _statisticService = statisticService;
            _notificationService = notificationService;
            _relationshipRepository = relationshipRepository;
            _mapper = mapper;
        }
        async public Task<GroupMember> Get(Guid id)
        {

            var result = _groupMemberRepository.Get(id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found groupmember !");
            }
        }
        /// <summary>
        /// Lấy danh sách thành viên nhóm
        /// </summary>
        /// <param name="gId">Id nhóm</param>
        /// <returns></returns>
        //async public Task<List<GroupMemberResponse>> GetGroupMemberByAchievements(Guid gId, int index, GroupMemberFilter filter)
        //{
        //    var tmp = _groupMemberRepository.Find(m => m.GroupID == gId && m.Status != MemberStatus.Request);
        //    if (tmp.Count() < 0)
        //        return new List<GroupMemberResponse>();
        //    var memberIDs = tmp.ToList().Select(gm => gm.GolferID);
        //    List<GroupMemberResponse> groupMemberResponses = new List<GroupMemberResponse>();
        //    foreach (var i in memberIDs)
        //    {
        //        GroupMemberResponseDetail groupMemberResponseDetail = new GroupMemberResponseDetail();
        //        groupMemberResponseDetail.Golfer = _golferRepository.Get(i);
        //        groupMemberResponseDetail.Statistics = _statisticService.GetGolferStatistics(i, DateRangeFilter.All, DateTime.Now, DateTime.Now);
        //        GroupMemberResponse groupMemberResponse = new GroupMemberResponse()
        //        {
        //            GolferID = groupMemberResponseDetail.Golfer.Id,
        //            Cover = groupMemberResponseDetail.Golfer.Cover,
        //            FirstName = groupMemberResponseDetail.Golfer.FirstName,
        //            LastName = groupMemberResponseDetail.Golfer.LastName,
        //            Handicap = groupMemberResponseDetail.Golfer.Handicap,
        //            //ScoreCardID = groupMemberResponseDetail.Statistics.BestRound==null?null: groupMemberResponseDetail.Statistics.BestRound.ID,
        //            //Stroke = groupMemberResponseDetail.Statistics.BestRound==null?0: groupMemberResponseDetail.Statistics.BestRound.Grosses,
        //            TotalMatches = groupMemberResponseDetail.Statistics.TotalMatches
        //        };
        //        if (groupMemberResponseDetail.Statistics.BestRound != null)
        //        {
        //            groupMemberResponse.ScoreCardID = groupMemberResponseDetail.Statistics.BestRound.ID;
        //            groupMemberResponse.Stroke = groupMemberResponseDetail.Statistics.BestRound.Grosses;

        //        }
        //        groupMemberResponses.Add(groupMemberResponse);
        //    }
        //    switch (filter)
        //    {
        //        case GroupMemberFilter.Handicap:
        //            return groupMemberResponses.Skip(index).Take(Const.PageAmount).OrderBy(gmr => gmr.Handicap).ToList();
        //        case GroupMemberFilter.TotalMatches:
        //            return groupMemberResponses.Skip(index).Take(Const.PageAmount).OrderByDescending(gmr => gmr.TotalMatches).ToList();

        //        case GroupMemberFilter.BestMatch:
        //            return groupMemberResponses.Skip(index).Take(Const.PageAmount).OrderBy(gmr => gmr.Stroke).ToList();

        //        default:
        //            throw new BadRequestException("Filter not valid!");
        //    }
        //}

        public List<GroupMemberResponse> GetMemberByType(Guid currentID, Guid gID, GroupMemberFilterByType filter, GroupMemberOderByType oderByType,string searchKey ,int startIndex)
        {
            IEnumerable<GroupMember> groupMembers;
            switch (filter)
            {
                case GroupMemberFilterByType.Admin:
                    {
                        groupMembers = _groupMemberRepository.Find(m =>(m.Golfer.FirstName.Trim().ToLower().Contains(searchKey)|| m.Golfer.LastName.Trim().ToLower().Contains(searchKey)) && m.GroupID == gID && (m.Status == MemberStatus.Admin || m.Status == MemberStatus.Moderator));
                        break;

                    }
                case GroupMemberFilterByType.Member:
                    {
                        groupMembers = _groupMemberRepository.Find(m => (m.Golfer.FirstName.Trim().ToLower().Contains(searchKey) || m.Golfer.LastName.Trim().ToLower().Contains(searchKey)) && m.GroupID == gID && m.Status == MemberStatus.Member);
                        break;
                    } 
                case GroupMemberFilterByType.Request:
                    {
                        groupMembers = _groupMemberRepository.Find(m => (m.Golfer.FirstName.Trim().ToLower().Contains(searchKey) || m.Golfer.LastName.Trim().ToLower().Contains(searchKey)) && m.GroupID == gID && m.Status == MemberStatus.Request);
                        break;
                    }

                default:
                    {
                        throw new BadRequestException("Invalid data");
                    }
            }
            if (groupMembers.Count() < 0)
                return new List<GroupMemberResponse>(); 
            List<GroupMemberResponse> groupMemberResponses = new List<GroupMemberResponse>();
            foreach (var i in groupMembers.ToList())
            {
                GroupMemberResponseDetail groupMemberResponseDetail = new GroupMemberResponseDetail();
                groupMemberResponseDetail.Golfer = _golferRepository.Get(i.GolferID);
                groupMemberResponseDetail.Statistics = _statisticService.GetGolferStatistics(i.GolferID, DateRangeFilter.All, DateTime.Now, DateTime.Now);
                GroupMemberResponse groupMemberResponse = new GroupMemberResponse();
                groupMemberResponse.Golfer = _mapper.Map<MinimizedGolfer>(groupMemberResponseDetail.Golfer);
                groupMemberResponse.TotalMatches = groupMemberResponseDetail.Statistics.TotalMatches;
                groupMemberResponse.MemberType = i.Status;
                groupMemberResponses.Add(groupMemberResponse);
            }
            switch (oderByType)
            {
                case GroupMemberOderByType.HDCIncrease:
                    {
                        groupMemberResponses = groupMemberResponses.OrderBy(m => m.Golfer.Handicap).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case GroupMemberOderByType.HDCDecrease:
                    {
                        groupMemberResponses = groupMemberResponses.OrderByDescending(m => m.Golfer.Handicap).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case GroupMemberOderByType.TotalMatchIncrease:
                    {
                        groupMemberResponses = groupMemberResponses.OrderBy(m => m.TotalMatches).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                case GroupMemberOderByType.TotalMatchDecrease:
                    {
                        groupMemberResponses = groupMemberResponses.OrderByDescending(m => m.TotalMatches).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
                default:
                    {
                        groupMemberResponses = groupMemberResponses.OrderBy(m => m.MemberType).Skip(startIndex).Take(Const.PageSize).ToList();
                        break;
                    }
            }
            foreach (var i in groupMemberResponses)
            {
                i.Relationship = _relationshipService.GetRelationship(currentID, i.Golfer.ID);
            }
            return groupMemberResponses;
        }
        async public Task<List<MinimizedGolfer>> GetGroupMemberReqest(Guid gId, int index)
        {
            var tmp = _groupMemberRepository.Find(m => m.GroupID == gId && m.Status == MemberStatus.Request).Skip(index).Take(Const.PageSize).OrderByDescending(u => u.Status);
            var listGolfer = tmp.ToList();
            var listGolferId = listGolfer.Select(golfer => golfer.GolferID).ToList();
            if (listGolferId.Count() == 0)
            {
                return new List<MinimizedGolfer>();
            }
            else
            {
                //IEnumerable<Golfer> result = _golferRepository.Get(q => listGolferId.Contains(q.Id));
                var result = _golferService.GetMinimizedGolfers(listGolferId);
                return result;
            }
        }
        /// <summary>
        /// Đếm số thành viên nhóm
        /// </summary>
        /// <param name="gId"></param>
        /// <returns></returns>
        async public Task<int> CountGroupMember(Guid gId)
        {
            var result = _groupMemberRepository.Find(m => m.GroupID == gId && m.Status != MemberStatus.Request).Count();
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found groupmember !");
            }
        }
        /// <summary>
        /// Gửi yêu cầu vào nhóm
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="member"></param>
        /// <returns></returns>
        async public Task<bool> AddGroupMemberRequest(Guid uId, Guid gid)
        {
            GroupMember member = new GroupMember();
            member.GolferID = uId;
            member.GroupID = gid;
            member.Status = MemberStatus.Request;
            var group = _groupRepository.Get(gid);
            if (group == null)
            {
                throw new BadRequestException("this group don't exit");
            }
            var gmember = _groupMemberRepository.Find(gm => gm.GolferID == uId && gm.GroupID == gid).FirstOrDefault();
            if (gmember != null)
            {
                throw new BadRequestException("this member exit");
            }
            _groupMemberRepository.Add(member);
            await _notificationService.NotificationSendGroupMemberRequest(uId, gid);
            return true;
        }
        /// <summary>
        /// Thêm thành viên vào nhốm
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="group"></param>
        /// <param name="golferid"></param>
        /// <returns></returns>
        async public Task<bool> AddGroupMember(Guid uId, Group group, Guid golferid)
        {
            if (_relationshipService.IsFriend(uId, golferid) == false)
                throw new BadRequestException("Can't add member");
            var currentGroupMember = _groupMemberRepository.Find(gm => gm.GolferID == uId && gm.GroupID == group.ID).FirstOrDefault();
            GroupMember member = new GroupMember();
            member.GolferID = golferid;
            member.GroupID = group.ID;
            var groupmember = _groupMemberRepository.Find(gm => gm.GroupID == group.ID && gm.GolferID == golferid).FirstOrDefault();
            if (groupmember != null && groupmember.Status != MemberStatus.Request)
                throw new BadRequestException("Member already exists!");
            var groupAdmins = _groupMemberRepository.Find(gm => gm.GroupID == group.ID && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).ToList();
            if (groupAdmins.Count() > 0 || uId == group.OwnerID)
            {
                if (groupAdmins.Select(ga => ga.ID).ToList().Contains(uId) || group.OwnerID == uId)
                {
                    if (groupmember != null && groupmember.Status == MemberStatus.Request)
                    {
                        groupmember.Status = MemberStatus.Member;
                        _groupMemberRepository.UpdateEntity(groupmember);
                    }
                    else if (groupmember == null)
                    {
                        member.Status = MemberStatus.Member;
                        _groupMemberRepository.Add(member);
                    }
                    //make notification
                    _notificationService.NotificationAddGroupMember(uId, golferid, group.ID);
                    return true;
                }
                else if(currentGroupMember!=null&&currentGroupMember.Status==MemberStatus.Member)
                {
                    if(group.Type==GroupType.Public)
                    {
                        if (groupmember != null && groupmember.Status == MemberStatus.Request)
                        {
                            groupmember.Status = MemberStatus.Member;
                            _groupMemberRepository.UpdateEntity(groupmember);
                            _notificationService.NotificationConfirmGroupMember(uId, golferid, group.ID);
                        }
                        else if (groupmember == null)
                        {
                            member.Status = MemberStatus.Member;
                            _groupMemberRepository.Add(member);
                            _notificationService.NotificationAddGroupMember(uId, golferid, group.ID);
                        }
                        //make notification
                        return true;
                    }
                    else
                    {
                        if (groupmember != null && groupmember.Status == MemberStatus.Request)
                        {
                            return true;
                        }
                        else if (groupmember == null)
                        {
                            member.Status = MemberStatus.Request;
                            _groupMemberRepository.Add(member);
                            _notificationService.NotificationInviteGroupMember(uId, golferid, group.ID);
                        }
                        //make notification
                        return true;
                    } 
                        
                }
                return false;
            }
            else
            {
                throw new BadRequestException("Not allow!");
            }
        }
        async public Task SafeAddGroupMember(Guid uId, Group group, Guid golferid, bool requireIsFriend)
        {
            GroupMember member = new GroupMember();
            member.GolferID = golferid;
            member.GroupID = group.ID;
            if (requireIsFriend)
            {
                if (_relationshipService.IsFriend(uId, golferid) == false)
                    return;
            }
            var groupmember = _groupMemberRepository.Find(gm => gm.GroupID == group.ID && gm.GolferID == golferid).FirstOrDefault();

            if ((groupmember != null && groupmember.Status != MemberStatus.Request) || golferid == group.OwnerID)
                return;
            var groupAdmins = _groupMemberRepository.Find(gm => gm.GroupID == group.ID && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).ToList();
            if (groupAdmins.Count() > 0 || uId == group.OwnerID)
            {
                if (groupAdmins.Select(ga => ga.ID).ToList().Contains(uId) || uId == group.OwnerID)
                {
                    if (groupmember != null && groupmember.Status == MemberStatus.Request)
                    {
                        groupmember.Status = MemberStatus.Member;
                        _groupMemberRepository.UpdateEntity(groupmember);
                    }
                    else
                    {
                        member.Status = MemberStatus.Member;
                        _groupMemberRepository.SafeAdd(member);
                    }
                    //make notification
                    _notificationService.NotificationAddGroupMember(uId, golferid, group.ID);
                    return;
                }
                else
                {
                    throw new BadRequestException("Can't add member");
                }
            }
            else
            {
                throw new BadRequestException("Not allow!");
            }
        }

         public async Task<bool> SetGroupAdmin(Guid uId, Guid groupid,Guid golferID)
        {
            var member = _groupMemberRepository.Find(gm => gm.GolferID == golferID && gm.GroupID == groupid && gm.Status != MemberStatus.Admin && gm.Status != MemberStatus.Request).FirstOrDefault();
            if(member==null)
            {
                throw new NotFoundException("Cann't set Admin");
            }    
            var groupAdmin = _groupMemberRepository.Find(gm => gm.GroupID == groupid && gm.Status == MemberStatus.Admin ).FirstOrDefault();
            if (groupAdmin == null)
            {
                throw new NotFoundException("Not found Admin");
            }
            else
            {
                if( uId==groupAdmin.GolferID)
                {
                    try
                    {
                        _databaseTransaction.BeginTransaction();
                        var group = _groupRepository.Get(groupid);
                        group.OwnerID = golferID;
                        groupAdmin.Status = MemberStatus.Member;
                        member.Status = MemberStatus.Admin;
                        _groupMemberRepository.SafeUpdate(groupAdmin);
                        _groupMemberRepository.SafeAdd(member);
                        _groupRepository.SafeUpdate(group);
                       await _databaseTransaction.Commit();
                        return true;
                    }   
                   catch(Exception e)
                    {
                        _databaseTransaction.Rollback();
                        throw new Exception(e.Message);
                    }
                }
                else
                {
                    throw new BadRequestException("Not allew!");
                }
            }

        }
        async public Task<GroupMember> AddGroupAdmin(Guid uId, Guid gid)
        {
            GroupMember member = new GroupMember();
            member.GolferID = uId;
            member.GroupID = gid;
            var groupAdmin = _groupMemberRepository.Find(gm => gm.GroupID == gid && gm.Status == MemberStatus.Admin ).FirstOrDefault();
            if (groupAdmin == null)
            {
                member.Status = MemberStatus.Admin;
                _groupMemberRepository.Add(member);
                //make notification
                return member;
            }
            else
            {
                throw new BadRequestException("Đã tồn tại admin");
            }

        }
        async public Task<GroupMember> SafeAddGroupAdmin(Guid uId, Guid gid)
        {
            GroupMember member = new GroupMember();
            member.GolferID = uId;
            member.GroupID = gid;
            var groupAdmin = _groupMemberRepository.Find(gm => gm.GroupID == gid && (gm.Status == MemberStatus.Admin)).FirstOrDefault();
            if (groupAdmin == null)
            {
                member.Status = MemberStatus.Admin;
                _groupMemberRepository.SafeAdd(member);
                //make notification
                return member;
            }
            else
            {
                throw new BadRequestException("Đã tồn tại admin");
            }

        }
        /// <summary>
        /// Xác nhận yêu cầu tham gia nhóm
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="gMemberID"></param>
        /// <returns></returns>
        async public Task<GroupMember> ConfirmRequest(Guid uId, Guid gid, Guid golferid)
        {
            var member = _groupMemberRepository.Find(gm => gm.GroupID == gid && gm.GolferID == golferid).FirstOrDefault();
            // GroupMember 
            // var groupAdminID = group.CreatedBy;
            var groupAdminID = _groupMemberRepository.Find(gm => gm.GroupID == gid && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).FirstOrDefault().GolferID;
            if (groupAdminID != null)
            {
                if (uId == groupAdminID)
                {
                    member.Status = MemberStatus.Member;
                    _groupMemberRepository.UpdateEntity(member);
                    return member;
                }
                else
                {
                    throw new BadRequestException("Not allow edit GruopMember !");
                }
            }
            {
                throw new BadRequestException("Not found group !");
            }
        }

        /// <summary>
        /// Cấp quyền kiểm duyệt nhóm 
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="gMemberID"></param>
        /// <returns></returns>
        async public Task<GroupMember> SetModerator(Guid uId, Guid golferid, Guid gid)
        {
            var member = _groupMemberRepository.Find(gm => gm.GolferID == golferid && gid == gm.GroupID&&gm.Status==MemberStatus.Request).FirstOrDefault();
            if (member == null)
            {
                throw new NotFoundException("Cann't set Moderator");
            }
            var groupAdminID = _groupMemberRepository.Find(gm => gm.GroupID == gid && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).FirstOrDefault().GolferID;
            if (groupAdminID != null && member.Status == MemberStatus.Member)
            {
                if (uId == groupAdminID)
                {
                    member.Status = MemberStatus.Moderator;
                    _groupMemberRepository.UpdateEntity(member);
                    //make notification
                    _notificationService.NotificationAddModerator(uId, golferid, gid);
                    return member;
                }
                else
                {
                    throw new BadRequestException("Not allow set moderator !");
                }
            }
            else
            {
                throw new BadRequestException("Not found group !");
            }
        }
        async public Task<GroupMember> RejectModerator(Guid uId, Guid golferid, Guid gid)
        {
            var member = _groupMemberRepository.Find(gm => gm.GolferID == golferid && gid == gm.GroupID&&gm.Status==MemberStatus.Moderator).FirstOrDefault();
            if (member == null)
            {
                throw new NotFoundException("Cann't set Moderator");
            }
            var groupAdminID = _groupMemberRepository.Find(gm => gm.GroupID == gid && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).FirstOrDefault().GolferID;
            if (groupAdminID != null && member.Status == MemberStatus.Member)
            {
                if (uId == groupAdminID)
                {
                    member.Status = MemberStatus.Moderator;
                    _groupMemberRepository.UpdateEntity(member);
                    //make notification
                    _notificationService.NotificationAddModerator(uId, golferid, gid);
                    return member;
                }
                else
                {
                    throw new BadRequestException("Not allow reject moderator !");
                }
            }
            else
            {
                throw new BadRequestException("Not found group !");
            }
        }

        /// <summary>
        /// Rơi nhóm hoặc loại thành viên nhóm
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<bool> Delete(Guid uId, Guid gid)
        {
            var result = _groupMemberRepository.Find(gm => gm.GroupID == gid && gm.GolferID == uId);
            if (result.Count() > 0)
            {
                var group = _groupRepository.Get(gid);
                if (group == null)
                {
                    throw new BadRequestException("Not found group !");
                }
                if (uId != group.OwnerID)
                {
                    _groupMemberRepository.RemoveEntity(result.FirstOrDefault());
                    return true;
                }
                else
                {
                    throw new BadRequestException("Admin cann't leave the group");
                }
            }
            else
            {
                throw new BadRequestException("Not found groupmember !");
            }
        }

        public async Task<bool> SetMember(Guid currentGolferID, Guid groupID,Guid golferID,GroupMemberAction action)
        {
            try 
            {
                _databaseTransaction.BeginTransaction();
                var group = _groupRepository.Get(groupID);
                if (group == null)
                {
                    throw new NotFoundException("Not found group");
                }
                var currentGroupMember = _groupMemberRepository.Find(gm => gm.GolferID == currentGolferID && gm.GroupID == group.ID).FirstOrDefault();
                var groupmember = _groupMemberRepository.Find(gm => gm.GolferID == golferID && gm.GroupID == groupID).FirstOrDefault();
                var groupAdmin = _groupMemberRepository.Find(gm => gm.Status == MemberStatus.Admin&&gm.GroupID == groupID).FirstOrDefault();
                if(currentGroupMember==null)
                    throw new ForbiddenException("Access denied!");
                switch (action)
                {
                    case GroupMemberAction.ConfirmRequest:
                        {
                            if (currentGroupMember.Status==MemberStatus.Admin|| currentGroupMember.Status == MemberStatus.Moderator)
                            {
                                if (groupmember != null && groupmember.Status == MemberStatus.Request)
                                {
                                    groupmember.Status = MemberStatus.Member;
                                    _groupMemberRepository.SafeUpdate(groupmember);
                                    //notificaton
                                    _notificationService.NotificationConfirmGroupMember(currentGolferID,golferID,groupID);
                                }
                                else
                                {
                                    throw new NotFoundException("Not found request!");
                                }
                            }
                            else
                            {
                                throw new ForbiddenException("Access denied!");
                            }
                            break;
                        }
                    case GroupMemberAction.AddMember:
                        {
                            if (_relationshipService.IsFriend(currentGolferID, golferID) == false)
                                throw new BadRequestException("Can't add member");
                            GroupMember member = new GroupMember();
                            member.GolferID = golferID;
                            member.GroupID = group.ID;
                                if (currentGroupMember.Status == MemberStatus.Admin || currentGroupMember.Status == MemberStatus.Moderator)
                                {
                                    if (groupmember != null && groupmember.Status == MemberStatus.Request)
                                    {
                                        groupmember.Status = MemberStatus.Member;
                                        _groupMemberRepository.UpdateEntity(groupmember);
                                    }
                                    else if (groupmember == null)
                                    {
                                        member.Status = MemberStatus.Member;
                                        _groupMemberRepository.Add(member);
                                    }
                                    //make notification
                                    _notificationService.NotificationAddGroupMember(currentGolferID, golferID, group.ID);
                                    return true;
                                }
                                else if (currentGroupMember != null && currentGroupMember.Status == MemberStatus.Member)
                                {
                                    if (group.Type == GroupType.Public)
                                    {
                                        if (groupmember != null && groupmember.Status == MemberStatus.Request)
                                        {
                                            groupmember.Status = MemberStatus.Member;
                                            _groupMemberRepository.UpdateEntity(groupmember);
                                            _notificationService.NotificationConfirmGroupMember(currentGolferID, golferID, group.ID);
                                        }
                                        else if (groupmember == null)
                                        {
                                            member.Status = MemberStatus.Member;
                                            _groupMemberRepository.Add(member);
                                            _notificationService.NotificationAddGroupMember(currentGolferID, golferID, group.ID);
                                        }
                                        //make notification
                                        return true;
                                    }
                                    else
                                    {
                                        if (groupmember != null && groupmember.Status == MemberStatus.Request)
                                        {
                                            return true;
                                        }
                                        else if (groupmember == null)
                                        {
                                            member.Status = MemberStatus.Request;
                                            _groupMemberRepository.Add(member);
                                            _notificationService.NotificationInviteGroupMember(currentGolferID, groupID, group.ID);
                                        }
                                        //make notification
                                        return true;
                                    }

                                }
                                return false;
                            break;
                        }
                    case GroupMemberAction.RejectModerator:
                        {
                            if (group.OwnerID== currentGolferID)
                            {
                                if (groupmember != null && groupmember.Status == MemberStatus.Moderator)
                                {
                                    groupmember.Status = MemberStatus.Member;
                                    _groupMemberRepository.SafeUpdate(groupmember);
                                    //notificaton
                                    _notificationService.NotificationRejectModerator(currentGolferID, golferID, groupID);
                                }
                                else
                                {
                                    throw new NotFoundException("Not found Moderator!");
                                }
                            }
                            else
                            {
                                throw new ForbiddenException("Access denied!");
                            }
                            break;
                        }
                    case GroupMemberAction.SetAdmin:
                        {
                            if (currentGolferID==group.OwnerID)
                            {
                                if (groupmember != null && groupmember.Status != MemberStatus.Request && groupmember.Status != MemberStatus.Admin)
                                {
                                    groupAdmin.Status = MemberStatus.Member;
                                    _groupMemberRepository.SafeUpdate(groupAdmin);
                                    groupmember.Status = MemberStatus.Admin;
                                    _groupMemberRepository.SafeUpdate(groupmember);
                                    group.OwnerID = golferID;
                                    _groupRepository.SafeUpdate(group);
                                    //notification
                                    await _notificationService.NotificationAddAdmin(currentGolferID, golferID, groupID);
                                }
                                else
                                {
                                    throw new NotFoundException("Not found member!");
                                }
                            }
                            else
                            {
                                throw new ForbiddenException("Access denied!");
                            }
                            break;
                        }
                    case GroupMemberAction.SetModerator:
                        {
                            if (currentGroupMember.Status == MemberStatus.Admin || currentGroupMember.Status == MemberStatus.Moderator)
                            {
                                if (groupmember != null && groupmember.Status == MemberStatus.Member)
                                {
                                    groupmember.Status = MemberStatus.Moderator;
                                    _groupMemberRepository.SafeUpdate(groupmember);
                                    //notification
                                    _notificationService.NotificationAddModerator(currentGolferID, golferID, groupID);
                                }
                                else
                                {
                                    throw new NotFoundException("Not found member!");
                                }
                            }
                            else
                            {
                                throw new ForbiddenException("Access denied!");
                            }
                            break;
                        }
                    case GroupMemberAction.DeleteMember:
                        {
                            if (currentGroupMember.Status == MemberStatus.Admin || currentGroupMember.Status == MemberStatus.Moderator)
                            {
                                if(groupmember != null && groupmember.Status != MemberStatus.Admin&& golferID == currentGolferID)
                                {
                                    _groupMemberRepository.SafeRemove(groupmember);
                                }    
                                else
                                {
                                    if (groupmember != null && (groupmember.Status == MemberStatus.Request|| groupmember.Status == MemberStatus.Member))
                                    {
                                        _groupMemberRepository.SafeRemove(groupmember);
                                        //notification
                                        _notificationService.NotificationRemoveGroupMember(currentGolferID, golferID, groupID);
                                    }
                                    else if (groupmember != null && groupmember.Status == MemberStatus.Moderator)
                                    {
                                        if (currentGolferID == group.OwnerID)
                                        {
                                            _groupMemberRepository.SafeRemove(groupmember);
                                            //notification
                                            _notificationService.NotificationRemoveGroupMember(currentGolferID, golferID, groupID);
                                        }
                                    }
                                    else
                                    {
                                        throw new NotFoundException("Not found request!");
                                    }
                                }
                               
                            }
                            else
                            {
                                throw new ForbiddenException("Access denied!");
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
               await _databaseTransaction.Commit();
                return true;
            }
            catch(Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);

            }
           
        }
    }
}
