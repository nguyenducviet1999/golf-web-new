using AutoMapper;
using Golf.Core.Dtos.Controllers.MembershipController.Responses;
using Golf.HttpApi.Host.Helpers;
using Golf.Services;
using Golf.Services.Memberships;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MembershipsController : ControllerBase
    {
        private readonly MembershipService _membershipService;
        private readonly IMapper _mapper;

        public MembershipsController(IMapper mapper, MembershipService membershipService)
        {
            _mapper = mapper;
            _membershipService = membershipService;
        }
        /// <summary>
        /// Lấy danh sách các gói membership
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("ListMembership/{startIndex}")]
        public async Task<ActionResult<List<OdooMembershipResponse>>> GetList(int startIndex)
        {
            return Ok(await _membershipService.GetOdooMembershipResponses( startIndex));
        }
        /// <summary>
        /// Lấy chị tiết gói membership
        /// </summary>
        /// <param name="membershipID">Định danh gói</param>
        /// <returns></returns>
        [HttpGet("{membershipID}")]
        public async Task<ActionResult<OdooMembershipResponse>> GetDetail(int membershipID)
        {
            return Ok(await _membershipService.GetOdooMembershipResponse(membershipID));
        }
        /// <summary>
        /// Mua gói membership
        /// </summary>
        /// <param name="membershipID">định danh gói membership</param>
        /// <returns></returns>
        [HttpPost("Buy/{membershipID}")]
        public async Task<ActionResult<string>> BuyMembership(int membershipID)
        {
            return Ok(await _membershipService.BuyOdooMembership(membershipID));
        }
        /// <summary>
        /// Danh sách các gói membership đã mua
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("MyMembership/{startIndex}")]
        public async Task<ActionResult<List<OdooMyMembershipResponse>>> MyMembership(int startIndex)
        {
            return Ok(await _membershipService.GetMyOdooMembership(startIndex));
        }
    }
}
