using AutoMapper;
using Golf.Core.Dtos.Controllers.GroupController.Requests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Golf.EntityFrameworkCore;
using Golf.Core.Dtos.Groups;
using Golf.Core.Exceptions;
using Golf.Domain.Shared.Resources;
using Golf.Domain.Shared;
using Golf.Domain.SocialNetwork;
using Golf.EntityFrameworkCore.Repositories;

using Golf.Core.Dtos.Controllers.GroupAdminController.Requests;
using Golf.Domain.Resources;
using Golf.Core.Dtos.Controllers.GroupController.Requests;
using Golf.Core.Common.Post;
using Golf.Domain.GolferData;
using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Microsoft.AspNetCore.Http;
using Golf.Domain.Post;
using Golf.Domain.Shared.Post;
using Golf.Domain.Tournaments;
using Golf.Domain.Shared.Tuanament;
using Golf.Domain.Shared.Groups;
using Golf.Services.Notifications;

namespace Golf.Services
{
    public class GroupService
    {
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly NotificationService _notificationService;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly GroupMemberService _groupMemberService;
        private readonly PostService _postService;
        private readonly PostRepository _postRepository;
        private readonly PhotoService _photoService;
        private readonly TournamentRepository _tournamentRepository;
        private readonly TournamentMemberRepository _tournamentMemberRepository;
        private readonly IMapper _mapper;
        private readonly RelationshipService _relationshipService;

        public GroupService(
            NotificationService notificationService,
            TournamentMemberRepository tournamentMemberRepository,
            TournamentRepository tournamentRepository,
            RelationshipService relationshipService,
            PostRepository postRepository,
            PostService postService,
            GroupMemberService groupMemberService,
            GroupMemberRepository groupMemberRepository,
            GroupRepository groupRepository,
            IMapper mapper,
            PhotoService photoService,
            DatabaseTransaction databaseTransaction
            )
        {
            _notificationService = notificationService;
            _tournamentMemberRepository = tournamentMemberRepository;
            _tournamentRepository = tournamentRepository;
            _relationshipService = relationshipService;
            _groupRepository = groupRepository;
            _mapper = mapper;
            _groupMemberRepository = groupMemberRepository;
            _groupMemberService = groupMemberService;
            _postService = postService;
            _postRepository = postRepository;
            _photoService = photoService;
            _databaseTransaction = databaseTransaction;
        }
        /// <summary>
        /// tạo nhóm mới
        /// </summary>
        /// <param name="uId">Định danh người dùng hiện thời</param>
        /// <param name="group">Dữ liệu nhóm</param>
        /// <param name="photoFile">File ảnh bìa nếu có</param>
        /// <param name="memberIDs">Danh sách định danh thành viên thêm vào</param>
        /// <returns></returns>
        public async Task<GroupResponse> Add(Guid uId, Group group, IFormFile photoFile, List<Guid> memberIDs)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                await this.SafeAdd(uId, group, photoFile, memberIDs, true);
                await _databaseTransaction.Commit();
                return this.GetDetail(uId, group.ID);
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        public async Task<bool> SafeAdd(Guid uId, Group group, IFormFile photoFile, List<Guid> memberIDs, bool requireIsFriend)
        {
            Photo photo = null;
            try
            {
                if (photoFile != null)
                {
                    photo = await _photoService.SafeSavePhoto(uId, photoFile, PhotoType.Group);
                }

                group.CreatedBy = uId;
                group.Cover = photo == null ? "" : $"{photo.Name}";
                _groupRepository.SafeAdd(group);
                await _groupMemberService.SafeAddGroupAdmin(uId, group.ID);
                foreach (var i in memberIDs)
                {
                    await _groupMemberService.SafeAddGroupMember(uId, group, i, requireIsFriend);
                }
                Post post = new Post();
                post.GroupID = group.ID;
                post.OwnerID = uId;
                post.PostAction = PostAction.CreateGroup;
                _postRepository.SafeAdd(post);
                return true;
            }
            catch (Exception e)
            {
                if (photo != null)
                {
                    _photoService.DeletePhoto(photo.Name);
                }
                throw new Exception(e.Message);
            }
        }


        public MemberStatus GetmemberStatus(Guid uId, Guid id)
        {
            var tmp = _groupMemberRepository.Find(gm => gm.GroupID == id && gm.GolferID == uId).FirstOrDefault();
            if (tmp != null)
            {
                return tmp.Status;
            }
            else
            {
                return MemberStatus.None;
            }
        }
        public bool IsMember(Guid uId, Guid id)
        {
            var tmp = _groupMemberRepository.Find(gm => gm.GroupID == id && gm.GolferID == uId && gm.Status != MemberStatus.Request).FirstOrDefault();
            if (tmp != null)
            {
                if (tmp.Status == MemberStatus.Request)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<PostResponse> GetPostGroup(Guid uId, Guid id, int index)
        {
            var memberStatus = this.GetmemberStatus(uId, id);
            if (_groupRepository.Get(id).Type == 0 || (memberStatus != MemberStatus.None && memberStatus != MemberStatus.Request))
            {
                List<PostResponse> listPost = new List<PostResponse>();
                var tmp = _postRepository.Find(p => p.GroupID == id).ToList().Skip(index).Take(Const.PageSize).OrderByDescending(p => p.CreatedDate).ToList();
                if (tmp.Count() > 0)
                {
                    foreach (var i in tmp)
                    {
                        var post = _postService.GetPostResponse(i, uId).Result;
                        if (post != null)
                        {
                            listPost.Add(post);
                        }
                    }
                    return listPost;
                }
                else
                {
                    return listPost;// throw new BadRequestException("Not found post group !");
                }
            }
            else
            {
                throw new BadRequestException("This golfer isn't member!");
            }
        }
        public async Task<List<PostResponse>> GetPinPostGroup(Guid currentGolferID, Guid groupID,int startIndex)
        {
            var groupmember = _groupMemberRepository.Find(gm => gm.Status != MemberStatus.Request && gm.GolferID == currentGolferID && gm.GroupID == groupID).FirstOrDefault();
            if (groupmember == null)
            {
                throw new ForbiddenException("Not allow!!!");
            }
            var pinPostGroup = _postRepository.Find(p => p.GroupID == groupID && p.IsPin == true).Skip(startIndex).Take(Const.PageSize).ToList();
            List<PostResponse> postresponses = new List<PostResponse>();
            foreach (var i in pinPostGroup)
            {
                postresponses.Add(await _postService.GetPostResponse(i, currentGolferID));
            }
            return postresponses;
        }
        public async Task<bool> PinPostGroup(Guid currentGolferID, Guid postID)
        {
            var post = _postRepository.Get(postID);
            if (post == null)
            {
                throw new NotFoundException("Not fount Post");
            }
            if (post.GroupID == null)
            {
                return false;
            }
            var groupAdmin = _groupMemberRepository.Find(gm => gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator).Select(gm => gm.GolferID).ToList();
            if (groupAdmin.Contains(currentGolferID) == false)
                throw new ForbiddenException("Not allow!!!");
            post.IsPin = true;
            _postRepository.UpdateEntity(post);
            return true;
        }
        public async Task<bool> UnPinPostGroup(Guid currentGolferID, Guid postID)
        {
            var post = _postRepository.Get(postID);
            if (post == null)
            {
                throw new NotFoundException("Not fount Post");
            }
            if (post.GroupID == null)
            {
                return false;
            }
            var groupAdmin = _groupMemberRepository.Find(gm => gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator).Select(gm => gm.GolferID).ToList();
            if (groupAdmin.Contains(currentGolferID) == false)
                throw new ForbiddenException("Not allow!!!");
            post.IsPin = null;
            _postRepository.UpdateEntity(post);
            return true;
        }

        public List<Group> GetListGroupByGolferId(Guid GolferID, int startIndex)
        {
            var listGroupID = _groupMemberRepository.Find(gm => gm.GolferID == GolferID && gm.Status != MemberStatus.Request).Select(gm => gm.GroupID).ToList();
            return _groupRepository.Find(group => listGroupID.Contains(group.ID)).Skip(startIndex).Take(Const.PageSize).ToList();
        }

        public List<GroupResponse> GetGroupsOfGolfer(Guid GolferID)
        {
            List<GroupResponse> groups = new List<GroupResponse>();
            var golferGroupMembers = _groupMemberRepository.Find(gm => gm.GolferID == GolferID && gm.Status != MemberStatus.Request).ToList();
            if (golferGroupMembers.Count() == 0)
            {
                return groups;
            }
            foreach (var i in golferGroupMembers)
            {
                //GroupResponse groupResponse = new GroupResponse();

                groups.Add(this.GetDetail(GolferID, i.GroupID));
            }
            // groups = _groupRepository.Find(g => golferGroupMembers.Select(gm => gm.GroupID).ToList().Contains(g.ID)).ToList();
            if (groups.Count() > 0)
            {
                return groups;
            }
            return new List<GroupResponse>();
        }
        public List<GroupResponse> GetGroups(Guid GolferID, GroupFilter groupFilter, string searchKey, int startIndex)
        {
            List<GroupResponse> groups = new List<GroupResponse>();
            List<GroupMember> golferGroupMembers;
            searchKey = searchKey.Trim().ToLower();
            switch (groupFilter)
            {
                case GroupFilter.MyGroup:
                    {
                        golferGroupMembers = _groupMemberRepository.Find(gm => gm.GolferID == GolferID && gm.Group.Name.Trim().ToLower().Contains(searchKey) && (gm.Status == MemberStatus.Admin || gm.Status == MemberStatus.Moderator)).Skip(startIndex).Take(Const.PageSize).ToList();
                        if (golferGroupMembers.Count() == 0)
                        {
                            return groups;
                        }
                        foreach (var i in golferGroupMembers)
                        {
                            groups.Add(this.GetDetail(GolferID, i.GroupID));
                        }
                        return groups;
                    }
                case GroupFilter.JoinedGroup:
                    {
                        golferGroupMembers = _groupMemberRepository.Find(gm => gm.GolferID == GolferID && gm.Group.Name.Trim().ToLower().Contains(searchKey) && gm.Status == MemberStatus.Member).Skip(startIndex).Take(Const.PageSize).ToList();
                        if (golferGroupMembers.Count() == 0)
                        {
                            return groups;
                        }
                        foreach (var i in golferGroupMembers)
                        {
                            groups.Add(this.GetDetail(GolferID, i.GroupID));
                        }
                        return groups;
                    }
                case GroupFilter.All:
                    {
                        var listGroup = _groupRepository.Find(g => g.Name.Trim().ToLower().Contains(searchKey)).Skip(startIndex).Take(Const.PageSize).ToList();
                        if (listGroup.Count() == 0)
                        {
                            return groups;
                        }
                        foreach (var i in listGroup)
                        {
                            groups.Add(this.GetDetail(GolferID, i.ID));
                        }
                        return groups.OrderByDescending(g => g.MemberStatus).ToList();
                    }
            }
            return new List<GroupResponse>();
        }

        public List<Guid> GetGroupIDsOfGolfer(Guid GolferID)
        {
            return GetGroupsOfGolfer(GolferID).Select(group => group.ID).ToList();
        }

        public GroupResponse GetDetail(Guid golferID, Guid id)
        {
            GroupResponse respone = new GroupResponse();
            var result = _groupRepository.Get(id);
            if (result != null)
            {
                return this.GetGroupResponse(golferID, result);
            }
            else
            {
                throw new BadRequestException("Not found group !");
            }
        }

        public List<GroupResponse> GetSearchGroupByName(Guid curentID, string name, int index)
        {
            var result = _groupRepository.Find(g => g.Name.Trim().ToLower().Contains(name)).Skip(index).Take(Const.PageSize).ToList();
            List<GroupResponse> listGroup = new List<GroupResponse>();
            if (result.Count() > 0)
            {
                foreach (var i in result)
                {
                    GroupResponse respone = new GroupResponse();
                    _mapper.Map(i, respone);
                    respone.NumberMember = _groupMemberService.CountGroupMember(i.ID).Result;
                    respone.MemberStatus = this.GetmemberStatus(curentID, i.ID);
                    listGroup.Add(respone);
                }

            }
            return listGroup.OrderByDescending(g => g.MemberStatus).ToList();
        }

        public GroupResponse Edit(Guid uId, Guid id, EditGroupRequest groupRequest)
        {
            var result = _groupRepository.Get(id);
            if (result == null)
            {
                throw new BadRequestException("Can not edit group !");
            }
            result.ID = id;
            _mapper.Map(groupRequest, result);
            _groupRepository.UpdateEntity(result);
            return this.GetDetail(uId, id); ;
        }


        public async Task<Group> EditGroupPhotos(Golfer currentGolfer, Guid groupID, SetGroupCoverRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var photo = await _photoService.SafeSavePhoto(groupID, request.GroupCover, PhotoType.Cover);
                var group = _groupRepository.Get(groupID);
                if (group != null && group.CreatedBy == currentGolfer.Id)
                {
                    var oldPhotoID = group.Cover;
                    group.Cover = photo.Name;
                    if (oldPhotoID != null && oldPhotoID != "")
                    {
                        _photoService.DeletePhoto((Guid)group.CreatedBy, oldPhotoID);

                    }
                    group.Cover = $"{photo.Name}";
                    await _databaseTransaction.Commit();
                    return _groupRepository.Get(groupID);
                }
                throw new ForbiddenException("Access Denied");
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Update course's photos error: {exception}");
            }
        }
        /// <summary>
        /// Xóa nhóm
        /// </summary>
        /// <param name="uId">Định danh người dùng hiện thời</param>
        /// <param name="groupID">Định danh nhóm</param>
        /// <returns></returns>
        public async Task<bool> DeleteGroup(Guid uId, Guid groupID, bool safe)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var group = _groupRepository.Get(groupID);
                if (safe)
                {
                    if (group.OwnerID != uId)
                    {
                        throw new ForbiddenException("Access Denied");
                    }
                }
                _groupRepository.SafeRemove(group);
                _groupMemberRepository.SafeRemoveRange(_groupMemberRepository.Find(gm => gm.GroupID == groupID).ToList());
                _postRepository.SafeRemoveRange(_postRepository.Find(p => p.GroupID == groupID).ToList());
                var members = _groupMemberRepository.Find(gm => gm.GroupID == groupID);
                await _databaseTransaction.Commit();
                foreach (var i in members)
                {
                    if (i.Status != MemberStatus.Request)
                    {
                        _notificationService.NotificationRemoveGroup(i.GolferID, groupID);
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception(exception.Message);
            }
        }
        public async Task SafeDeleteGroup(Guid uId, Guid groupID, bool safe)
        {

            var group = _groupRepository.Get(groupID);
            if (safe)
            {
                if (group.OwnerID == uId)
                {
                    throw new ForbiddenException("Access Denied");
                }
            }
            _groupRepository.SafeRemove(group);
            _groupMemberRepository.SafeRemoveRange(_groupMemberRepository.Find(gm => gm.GroupID == groupID));
        }
        public GroupResponse GetGroupResponse(Guid currentGolferID, Group group)
        {
            GroupResponse respone = new GroupResponse();
            _mapper.Map(group, respone);
            respone.NumberMember = _groupMemberService.CountGroupMember(group.ID).Result;
            respone.MemberStatus = this.GetmemberStatus(currentGolferID, group.ID);
            return respone;
        }
        public List<GroupResponse> GetGroupResponses(Guid currentGolferID, List<Group> groups)
        {
            List<GroupResponse> responses = new List<GroupResponse>();
            foreach (var i in groups)
            {
                GroupResponse respone = new GroupResponse();
                _mapper.Map(i, respone);
                respone.NumberMember = _groupMemberService.CountGroupMember(i.ID).Result;
                respone.MemberStatus = this.GetmemberStatus(currentGolferID, i.ID);
                responses.Add(respone);
            }
            return responses;
        }
        /// <summary>
        /// Tạo nhóm giải đấu
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="tournamentID"></param>
        /// <returns></returns>
        public async Task<GroupResponse> AddTournamentGroup(Guid currentID, Guid tournamentID)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                var tournament = _tournamentRepository.Get(tournamentID);
                if (tournament == null)
                {
                    throw new NotFoundException("Not found tournament!");
                }
                if (tournament.Status == TournamentStatus.Requested)
                {
                    throw new BadRequestException("Can not add tournament group!");
                }
                if (tournament.GroupID != null && tournament.GroupID != Guid.Empty)
                {
                    throw new BadRequestException("Group already exists");
                }
                var group = new Group();
                var memberIDs = new List<Guid>();
                group.Name = tournament.TournamentName;
                group.OwnerID = currentID;
                group.Type = GroupType.Private;
                _groupRepository.SafeAdd(group);
                var tournamentMember = _tournamentMemberRepository.Find(tm => tm.MemberStatus == TournamentMemberStatus.Joined && tm.TuornamentID == tournament.ID);
                if (tournamentMember.Count() > 0)
                {
                    memberIDs = tournamentMember.Select(tm => tm.GolferID).ToList();
                }
                await this.SafeAdd(currentID, group, null, memberIDs, false);
                tournament.GroupID = group.ID;
                _tournamentRepository.SafeUpdate(tournament);
                await _databaseTransaction.Commit();
                return this.GetDetail(currentID, group.ID);
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }

        }
    }
}



