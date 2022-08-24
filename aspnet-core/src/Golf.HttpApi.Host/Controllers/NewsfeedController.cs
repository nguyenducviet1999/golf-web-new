using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using Golf.HttpApi.Host.Helpers;
using Golf.Core.Common.Post;
using Golf.Domain.GolferData;
using Golf.Services;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class NewsfeedController : ControllerBase
    {
        private readonly NewsfeedService _newsFeedService;

        public NewsfeedController(NewsfeedService newsfeedService)
        {
            _newsFeedService = newsfeedService;
        }

        /// <summary>
        /// Lấy bài viết trong bảng tin
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns>Danh sách bài viết</returns>
        [HttpGet("{startIndex}")]
        public async Task<ActionResult<List<PostResponse>>> GetGolferNewsFeed(int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<PostResponse> postResponses = await _newsFeedService.GetGolferNewsfeed(currentGolfer.Id, startIndex);
            return postResponses;
        }

        /// <summary>
        /// lấy các bài viết của một người dùng 
        /// </summary>
        /// <param name="GolferID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Users/{GolferID}/{startIndex}")]
        public async Task<ActionResult<List<PostResponse>>> GetUserPosts(Guid GolferID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<PostResponse> postResponses = await _newsFeedService.GetGolferPosts(GolferID, currentGolfer.Id, startIndex);
            return postResponses;
        }

        /// <summary>
        /// Lấy bài viết nhóm
        /// </summary>
        /// <param name="GroupID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("Groups/{GroupID}/{startIndex}")]
        public async Task<ActionResult<List<PostResponse>>> GetGroupPosts(Guid GroupID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<PostResponse> postResponses = await _newsFeedService.GetGroupPosts(currentGolfer.Id, GroupID, startIndex);
            return postResponses;
        }

    }
}