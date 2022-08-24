using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

using Golf.Services;
using Golf.Domain.GolferData;
using Golf.Core.Common.Post;
using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Post;
using Golf.HttpApi.Host.Helpers;

using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.PostController.Requests;
using System.Linq;
using Golf.Domain.Shared.Scorecard;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Post;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostService _postService;
        private readonly ScorecardService _scorecardService;
        private readonly CommentService _commentService;
        private readonly PostVoteService _postVoteService;
        private readonly PostRepository _postRepository;

        public PostsController(
            PostRepository postRepository,
            CommentService commentService,
            ScorecardService scorecardService,
            PostService postService,
            PostVoteService postVoteService)
        {
            _postRepository = postRepository;
            _postService = postService;
            _scorecardService = scorecardService;
            _commentService = commentService;
            _postVoteService = postVoteService;
        }

        // GET: api/Posts/{PostID}
        /// <summary>
        /// Lấy bài viết chi tiết theo định danh
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <returns></returns>
        [HttpGet("{PostID}")]
        public async Task<ActionResult<PostResponse>> GetPost(Guid PostID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            PostResponse postResponse = await _postService.Get(PostID, currentGolfer);
            return Ok(postResponse);
        }

        // POST: api/Posts
        /// <summary>
        /// Tạo bài viết mới
        /// </summary>
        /// <param name="request">Dữ liệu tạo bài viết</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PostResponse>> CreatePost([FromForm] CreatePostRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            PostResponse postResponse = await _postService.Add(currentGolfer, request);
            return Ok(postResponse);
        }

        // PUT: api/Posts/{PostID}
        /// <summary>
        /// Sửa bài viết
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <param name="request">Dữ liệu chỉnh sửa</param>
        /// <returns></returns>
        [HttpPut("{PostID}")]
        public async Task<ActionResult<PostResponse>> EditPost(Guid PostID, [FromForm] EditPostRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            PostResponse postResponse = await _postService.Edit(PostID, currentGolfer, request);
            return Ok(postResponse);
        }


        // DELETE: api/Posts/{PostID}
        /// <summary>
        /// Xóa bài viết
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        [HttpDelete("{PostID}")]
        public ActionResult<bool> DeletePost(Guid PostID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var status = _postService.Remove(PostID, currentGolfer);
            return Ok(status.Result);
        }

        // POST: api/Posts/{PostID}/Share
        /// <summary>
        /// Chia sẻ bài viết
        /// </summary>
        /// <param name="PostID">Đinh danh bài viết</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{PostID}/Share")]
        public async Task<ActionResult<PostResponse>> SharePost(Guid PostID, [FromForm] SharePostRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            PostResponse postResponse = await _postService.Share(PostID, currentGolfer, request);
            return Ok(postResponse);
        }

        // POST: api/Posts/Scorecards
        /// <summary>
        /// Đăng bài viết kèm với bảng điểm
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Scorecards")]
        public async Task<ActionResult<CommentResponse>> SaveScorecards(SaveScorecardsRequest request)
        {
            if (request.Validate() == false)
            {
                throw new BadRequestException("Validate request failed");
            }
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            foreach (var i in request.Scorecards)
            {
                if (currentGolfer.Id == i.OwnerID)
                {
                    i.Type = ScorecardType.Posted;
                }
                else
                {
                    i.Type = ScorecardType.Pending;
                }

            }
            PostResponse postResponse = await _postService.SaveScorecards(currentGolfer, request);
            return Ok(postResponse);
        }
        [HttpPost("{postID}/InviteConfirm")]
        public async Task<ActionResult> Inviteconfirm(Guid postID,[FromForm]List<Guid> friendID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            _postService.InviteConfirmScorecard(currentGolfer.Id, postID, friendID);
            return Ok();
        }

        // POST: api/Posts/{PostID}/Comment
        /// <summary>
        /// Bfinh luận bài viết
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{PostID}/Comment")]
        public async Task<ActionResult<CommentResponse>> AddComment(Guid PostID, [FromForm] AddCommentRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var comment = await _commentService.AddComment(PostID, currentGolfer, request);
            //_postVoteService.AddPostVote(PostID, currentGolfer.Id, VoteType.Comment);
            CommentResponse commentResponse = _commentService.GetComment(comment, currentGolfer);
            return Ok(commentResponse);
        }

        /// <summary>
        /// Xác nhận bảng điểm
        /// </summary>
        /// <param name="PostID"></param>
        /// <returns></returns>
        /// 
        //[HttpPost("{PostID}/Confirm")]
        //public async Task<ActionResult<bool>> AddConfirm(Guid PostID)
        //{
        //    var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
        //    var post = _postRepository.Get(PostID);
        //    if (post.ScorecardIDs().Count() == 0)
        //    {
        //        throw new BadRequestException("Scorecard isn't exit");
        //    }
        //    // _postVoteService.AddPostVote(PostID, currentGolfer.Id, VoteType.Confirm);
        //    foreach (var i in post.ScorecardIDs())
        //    {
        //        //_postService.
        //        _scorecardService.ConfirmScorecard(i, currentGolfer);
        //    }
        //    return Ok(true);
        //}

        // PUT: api/Posts/{PostID}/Comment/{CommentID}
        /// <summary>
        /// Sửa nội dung bình luận
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <param name="CommentID">Định danh bình luận</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{PostID}/Comments/{CommentID}")]
        public async Task<ActionResult<CommentResponse>> EditComment(Guid PostID, Guid CommentID, [FromForm] EditCommentRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            CommentResponse commentResponse = await _commentService.EditComment(PostID, CommentID, currentGolfer, request);
            return Ok(commentResponse);
        }

        // DELETE: api/Posts/{PostID}/Comments/{CommentID}
        /// <summary>
        /// Xóa bình luận 
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="CommentID"></param>
        /// <returns></returns>
        [HttpDelete("{PostID}/Comments/{CommentID}")]
        public ActionResult<bool> DeleteComment(Guid PostID, Guid CommentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            bool status = _commentService.RemoveComment(PostID, CommentID, currentGolfer);
            return Ok(status);
        }

        // POST: api/Posts/{PostID}/Comments/{CommentID}/Reply
        /// <summary>
        /// Trả lời bình luận trong bài viết
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <param name="CommentID">Định danh bình luận</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{PostID}/Comments/{CommentID}/Reply")]
        public async Task<ActionResult<CommentDetailResponse>> ReplyComment(Guid PostID, Guid CommentID, [FromForm] AddCommentRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            CommentDetailResponse commentResponse = await _commentService.ReplyComment(PostID, CommentID, currentGolfer, request);
            //_postVoteService.AddCommentVote(PostID, CommentID, currentGolfer.Id, VoteType.Reply);
            return Ok(commentResponse);
        }

        // POST: api/Posts/{PostID}
        /// <summary>
        /// like, confirm scorrercard
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <param name="voteType">Loại Vote</param>
        /// <returns></returns>
        [HttpPost("{PostID}/{voteType}")]
        public ActionResult<bool> VotePost(Guid PostID,VoteType voteType)
        {
            Golfer currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            bool status = _postVoteService.AddPostVote(PostID, currentGolfer.Id, voteType);
            return Ok(status);
        }

        // POST: api/Posts/{PostID}/Comments/{CommentID}/Like
        /// <summary>
        /// Thích bình luận bài viết
        /// </summary>
        /// <param name="PostID">Định danh bài viêt</param>
        /// <param name="CommentID">Định danh bình luận</param>
        /// <returns></returns>
        [HttpPost("{PostID}/Comments/{CommentID}/Like")]
        public ActionResult<bool> LikeComment(Guid PostID, Guid CommentID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            bool status = _postVoteService.LikeComment(PostID, CommentID, currentGolfer.Id);
            return Ok(status);
        }

        // GET: api/Posts/{PostID}/Comments/{startIndex}
        /// <summary>
        /// Lấy bình luận bài viết
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{PostID}/Comments/{startIndex}")]
        public ActionResult<List<CommentDetailResponse>> GetPostComments(Guid PostID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<CommentDetailResponse> commentDetailResponses = _commentService.GetPostComments(PostID, currentGolfer, startIndex);
            return Ok(commentDetailResponses);
        }

        // GET: api/Posts/{PostID}/Comments/{CommentID}/Detail/{startIndex}
        /// <summary>
        /// lấy chi tiết 1 bình luận
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="CommentID"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{PostID}/Comments/{CommentID}/Detail/{startIndex}")]
        public ActionResult<CommentDetailResponse> GetPostComment(Guid PostID, Guid CommentID, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            CommentDetailResponse commentResponse = _commentService.GetCommentDetail(CommentID, currentGolfer, startIndex);
            return Ok(commentResponse);
        }

        /// <summary>
        /// Lấy danh sach người dùng vote bình luận
        /// </summary>
        /// <param name="PostID">ĐỊnh danh bài viết</param>
        /// <param name="CommentID">Điịnh danh bình luận</param>
        /// <param name="voteType"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{PostID}/Comments/{CommentID}/{voteType}/Detail/{startIndex}")]
        public ActionResult<List<MinimizedGolfer>> GetVoteOfComment(Guid PostID, Guid CommentID, GetDetailCommentVoteTypeRequest voteType, int startIndex)
        {
            Golfer currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<MinimizedGolfer> golfers = _commentService.GetDetailVoteTypeOfComment(PostID, CommentID, voteType, startIndex);
            return Ok(golfers);
        }

        // GET: api/Posts/{PostID}/{voteType}/Detail/{startIndex}
        /// <summary>
        /// Lấy danh sach người dùng vote bài viết
        /// </summary>
        /// <param name="PostID">ĐỊnh danh bài viết</param>
        /// <param name="voteType"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{PostID}/{voteType}/Detail/{startIndex}")]
        public ActionResult<List<MinimizedGolfer>> GetVoteOfPost(Guid PostID, GetDetailPostVoteTypeRequest voteType, int startIndex)
        {
            Golfer currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            List<MinimizedGolfer> golfers = _postService.GetDetailVoteTypeOfPost(PostID, voteType, startIndex);
            return Ok(golfers);
        }

        // GET: api/Posts/{PostID}/AllVotes
        /// <summary>
        /// Lấy vote bài viết
        /// </summary>
        /// <param name="PostID">Định danh bài viết</param>
        /// <returns></returns>
        [HttpGet("{PostID}/AllVotes")]
        public ActionResult<VotesOfPostResponse> GetAllVoteOfPost(Guid PostID)
        {
            Golfer currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            VotesOfPostResponse postResponse = _postVoteService.GetVotesOfPost(PostID, currentGolfer.Id);
            return Ok(postResponse);
        }
       
    }
}
