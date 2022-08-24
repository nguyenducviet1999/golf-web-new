
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.GroupAdminController.Requests;
using Golf.Core.Dtos.Controllers.GroupController.Requests;
using Golf.Core.Dtos.Groups;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Domain.Resources;
using Golf.Domain.Shared.Groups;
using Golf.Domain.Shared.Resources;
using Golf.Domain.SocialNetwork;
using Golf.EntityFrameworkCore.Repositories;
using Golf.HttpApi.Host.Helpers;
using Golf.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupAdminController : ControllerBase
    {
        private readonly GroupService _groupService;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberService _groupMemberService;
        private readonly PhotoService _photoService;
        public GroupAdminController(GroupRepository groupRepository, PhotoService photoService, GroupService groupService, GroupMemberService groupMemberService)
        {
            _groupRepository = groupRepository;
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _photoService = photoService;
        }


        /// <summary>
        /// Lấy danh sách yêu cầu tham gia nhóm 
        /// </summary>
        /// <param name="GroupID">Định danh nhóm</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{GroupID}/Members/Request/{startIndex}")]
        public ActionResult<List<MinimizedGolfer>> GetGroupMemberRequest(Guid GroupID, int startIndex)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var group = _groupRepository.Get(GroupID);
            if (group == null)
                throw new NotFoundException("Not found group");
            if (group.CreatedBy != golfer.Id)
            {
                throw new ForbiddenException("Not Allow!");
            }
            var result = _groupMemberService.GetGroupMemberReqest(GroupID, startIndex);
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result.Result.ToList());
            }
            else
            {
                return BadRequest(result.Exception.Message);
            }
        }

        /// <summary>
        /// Sửa thông tin nhóm
        /// </summary>
        /// <param name="GroupID">Định danh nhóm</param>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpPut("{GroupID}")]
        public ActionResult<GroupResponse> Put(Guid GroupID, [FromBody] EditGroupRequest group)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupService.Edit(golfer.Id, GroupID, group);
            return Ok(result);
        }

        /// <summary>
        /// Thêm người kiểm duyệt
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="groupMember"></param>
        /// <returns></returns>
        [HttpPut("{GroupID}/AddModerator/{GolferID}")]
        public ActionResult<GroupMember> SetcourseModerator(Guid GroupID, Guid GolferID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupMemberService.SetModerator(golfer.Id, GolferID, GroupID);
            return Ok(result.Result);
        }

        /// <summary>
        /// xóa nhóm
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        [HttpDelete("{GroupID}")]
        public async Task<ActionResult<GroupMember>> DeleteGroup(Guid GroupID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = await _groupService.DeleteGroup(golfer.Id, GroupID, true);
            return Ok(result);
        }
        [HttpPost("{groupID}/GroupMember/{groupAction}/{golferID}")]
        public async Task<ActionResult<bool>> SetMember(Guid groupID, Guid golferID, GroupMemberAction groupAction)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = await _groupMemberService.SetMember(golfer.Id, groupID, golferID, groupAction);
            return Ok(result);
        }

        /// <summary>
        /// ủy quyền admin
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="GolferID"></param>
        /// <returns></returns>
        [HttpPut("{GroupID}/SetAdmin/{GolferID}")]
        public ActionResult<GroupMember> SetGroupAdmin(Guid GroupID, Guid GolferID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupMemberService.SetGroupAdmin(golfer.Id, GroupID, GolferID);
            return Ok(result.Result);
        }
        /// <summary>
        /// Thêm những thành viên nhóm
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="golferid"></param>
        /// <returns></returns>
        [HttpPost("{groupID}/AddMember")]
        public async Task<ActionResult<bool>> AddGroupMember(Guid groupID, [FromBody] AddGroupMembersRequest request)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var group = _groupRepository.Get(groupID);
            if (group == null)
                throw new NotFoundException("Not founf group");
            foreach (var i in request.GolferIDs)
            {
                await _groupMemberService.AddGroupMember(golfer.Id, group, i);
            }
            return Ok(true);
        }

        /// <summary>
        /// duyệt thành viên nhóm
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="golferid"></param>
        /// <returns></returns>
        [HttpPut("{GroupID}/{golferid}/confirm")]
        public ActionResult<GroupMember> Confirm(Guid GroupID, Guid golferid)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupMemberService.ConfirmRequest(golfer.Id, GroupID, golferid);
            return Ok(result.Result);
        }

        //6/3/2021
        /// <summary>
        /// Đặt ảnh bìa nhóm
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{GroupID}/Cover")]
        async public Task<ActionResult<Group>> SetGroupCover(Guid GroupID, [FromForm] SetGroupCoverRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var rersult = await _groupService.EditGroupPhotos(currentGolfer, GroupID, request);
            return Ok(rersult);
        }

        /// <summary>
        /// Loại thành viên khỏi group
        /// </summary>
        /// <param name="GroupID">GroupID group</param>
        /// <param name="golferid">GroupID thành viên</param>
        /// <returns></returns>
        [HttpDelete("Leave/{GroupID}")]
        public ActionResult<bool> Delete(Guid GroupID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _groupMemberService.Delete(golfer.Id, GroupID);
            return Ok(result.Result);
        }
        //post
        [HttpPut("Posts/{postID}/Pin")]
        public async Task<ActionResult<bool>> PinGroupPost(Guid postID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return await _groupService.PinPostGroup(golfer.Id, postID);
        }
        [HttpPut("Posts/{postID}/UnPin")]
        public async Task<ActionResult<bool>> UnPinGroupPost(Guid postID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            return await _groupService.UnPinPostGroup(golfer.Id, postID);
        }
    }
}
