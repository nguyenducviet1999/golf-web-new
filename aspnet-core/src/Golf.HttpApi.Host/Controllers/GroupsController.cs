using AutoMapper;
using Golf.Core.Dtos.Controllers.GroupController.Requests;
using Golf.Core.Dtos.Groups;
using Golf.Domain.Shared;
using Golf.Domain.SocialNetwork;
using Golf.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Golf.Core.Dtos.Controllers.GroupController.Response;
using Golf.Domain.Shared.Resources;
using Golf.Domain.GolferData;
using Golf.HttpApi.Host.Helpers;
using Golf.Core.Common.Post;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Domain.Shared.Groups;
using Golf.EntityFrameworkCore.Repositories;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly GroupService _groupService;
        private readonly GroupMemberService _groupMemberService;
        private readonly RelationshipService _relationshipService;
        private readonly GolferService _golferService;
        private readonly GroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public GroupsController(RelationshipService relationshipService,GroupRepository groupRepository, GolferService golferService, IMapper mapper, GroupService groupService, GroupMemberService groupMemberService)
        {
            _relationshipService = relationshipService;
            _groupRepository = groupRepository;
            _golferService = golferService;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy bài viết của nhóm 
        /// </summary>
        /// <param name="GroupID">Định danh nhóm</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GroupID}/Posts/{startIndex}")]
        public ActionResult<IEnumerable<PostResponse>> GetPost(Guid GroupID, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupService.GetPostGroup(golfer.Id, GroupID, startIndex);
            return Ok(result);

        } 
        /// <summary>
        /// Lấy các bài viết được ghim
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GroupID}/PinPosts/{startIndex}")]
        public async Task<ActionResult<List<PostResponse>>> GetPinPost(Guid GroupID, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result =await _groupService.GetPinPostGroup(golfer.Id, GroupID, startIndex);
            return Ok(result);

        }
        /// <summary>
        /// Lấy dữ liệu thành viên nhóm
        /// </summary>
        /// <param name="groupID">ĐỊnh danh nhóm</param>
        /// <param name="type">loại thành viên</param>
        /// <param name="oderByType">Sắp xếp</param>
        /// <param name="startIndex">vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("{groupID}/{type}/{oderByType}/{startIndex}")]
        public ActionResult<List<MinimizedGolferResponse>> GetMemberByType(Guid groupID, GroupMemberFilterByType type, GroupMemberOderByType oderByType, string? searchKey,int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var memberStatus = _groupService.GetmemberStatus(golfer.Id, groupID);
            var group = _groupRepository.Get(groupID);
            if(group==null)
            {
                throw new NotFoundException("Not found group");
            }
            if ((memberStatus == MemberStatus.None || memberStatus == MemberStatus.Request)&&group.Type==GroupType.Private)
                throw new ForbiddenException("Access Denied!");
            if (memberStatus != MemberStatus.Admin && type == GroupMemberFilterByType.Request)
                throw new ForbiddenException("Access Denied!");
            var result = _groupMemberService.GetMemberByType(golfer.Id, groupID, type, oderByType, searchKey==null?"":searchKey, startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm nhóm theo tên
        /// </summary>
        /// <param name="key"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Search/{startIndex}")]
        public ActionResult<GroupResponse> SearchGroupByName([FromQuery] string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupService.GetSearchGroupByName(golfer.Id, searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex);
            return Ok(result);
        } 
        [HttpGet("SearchFriendToAddIntoGroup/groupID/{startIndex}")]
        public ActionResult<GroupResponse> SearchFriendToAddIntoGroup(Guid groupID, [FromQuery] string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _relationshipService.SearchFriendToAddIntoGroup(groupID, golfer.Id, searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách các nhóm đã tham gia của người dùng hiện tại
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("MyGroups/{startIndex}")]
        public ActionResult<List<GroupResponse>> GetMyGroups(int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupService.GetGroupsOfGolfer(golfer.Id).Skip(startIndex).Take(Const.PageSize).ToList();
            if (result == null)
            {
                return Ok(new List<GroupResponse>());
            }
            else
            {
                return result;
            }
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupFilter"></param>
        /// <param name="searchKey"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{groupFilter}/{startIndex}")]
        public ActionResult<List<GroupResponse>> GetGroups(GroupFilter groupFilter,[FromQuery] string searchKey, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return _groupService.GetGroups(golfer.Id, groupFilter, searchKey==null?"":searchKey, startIndex); 
            
        }

        [HttpGet("{GroupID}/detail")]
        public ActionResult<GroupResponse> GetDetail(Guid GroupID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return _groupService.GetDetail(golfer.Id, GroupID);
        }

        // POST api/<GroupsController>
        /// <summary>
        /// Tạo nhóm mới
        /// </summary>
        /// <param name="groupRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<GroupResponse> Post([FromForm] GroupRequest groupRequest)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            Group group = new Group();
            _mapper.Map(groupRequest, group);
            group.OwnerID = golfer.Id;
            var result = _groupService.Add(golfer.Id, group,groupRequest.PhotoFile,groupRequest.MemberIDs);
            return Ok(result.Result);
        }
        
        /// <summary>
        /// Gửi yêu cầu tham  gia nhóm
        /// </summary>
        /// <param name="groupID">Định danh nhóm</param>
        /// <returns></returns>
        [HttpPost("Request")]
        async public Task<ActionResult<bool>> RequestToJoin(Guid groupID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupMemberService.AddGroupMemberRequest(golfer.Id, groupID);
            return Ok(result.Result);
        }

        //group setting
        [HttpGet("{groupID}/Setting")]
        public ActionResult<GroupSetting> GroupSetting(Guid groupID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var group = _groupService.GetDetail(golfer.Id, groupID);
            if (_groupService.IsMember(golfer.Id, groupID))
            {
                throw new BadRequestException("Not Allow");
            }
            return Ok(_golferService.MyGroupSetting(golfer.Id, groupID));
        }

        [HttpPut("{groupID}/Setting")]
        public ActionResult<GroupSetting> GroupSetting(Guid groupID, [FromBody] GroupSetting groupSetting)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var group = _groupService.GetDetail(golfer.Id, groupID);
            if (_groupService.IsMember(golfer.Id, groupID))
            {
                throw new BadRequestException("Not Allow");
            }
            groupSetting.GroupID = groupID;
            return Ok(_golferService.UpdateMyGroupSetting(golfer.Id, groupSetting));
        }
    }
}
