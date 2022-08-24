// using Golf.Core.Dtos.Post;
﻿// using AutoMapper;
// using Golf.Core.Dtos.Controllers.CommentController.Request;
// using Golf.Core.Dtos.Post;
// using Golf.Domain.Shared;
// using Golf.Domain.SocialNetwork;
// using Golf.Domain.Golfer;
// using Golf.Services;
// using Golf.Services.Posts;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// // // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// namespace Golf.HttpApi.Host.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class CommentsController : ControllerBase
//     {
//         CommentService _commentService;
//         PostService _postService;
//         PostVoteService _postVoteService;
//         IMapper _mapper;
//         public CommentsController(IMapper mapper,CommentService commentService,PostService postService, PostVoteService postVoteService )
//         {
//             _commentService = commentService;
//             _postVoteService = postVoteService;
//             _postService = postService;
//             _mapper = mapper;
//         }
//         // GET: api/<CommentsController>



//         // POST api/<CommentsController>
//         [HttpPost]
//         async public Task<ActionResult> Post([FromBody] AddCommentRequest comment)
//         {
//            var golfer = (Golfer)HttpContext.Items["Golfer"];
//             Comment para = new Comment();
//             _mapper.Map(comment, para);
//             var result = _commentService.Add(golfer.Id, para);
//             if (result != null)
//             {
//                 return Ok(result.Result);
//             }
//             else return BadRequest(result.Exception.Message);
//         }

//         // PUT api/<CommentsController>/5
//         [HttpPut("{id}")]
//         async public Task<ActionResult> Put(Guid id, [FromBody] EditCommentRequest comment)
//         {
//             Comment para = new Comment();
//             _mapper.Map(comment, para);
//             para.ID = id;
//             var golfer = (Golfer)HttpContext.Items["Golfer"];
//             var result = _commentService.Edit(golfer.Id, para);
//             if (result != null)
//             {
//                 return Ok(result.Result);
//             }
//             else return BadRequest(result.Exception.Message);
//         }
//         [HttpPost("{id}/like")]
//         async public Task<ActionResult> LikeComment(Guid id)
//         {
//             var golfer = (Golfer)HttpContext.Items["Golfer"];
//             var post = _postService.Get(id);
//             if (post == null)
//             {
//                 return BadRequest();
//             }
//             PostVote pv = new PostVote();
//             pv.CommentID = id;
//             pv.GolferID = golfer.Id;
//             pv.VoteType = VoteType.Like;
//             var postVotes = _postVoteService.GetCommentVote(golfer.Id, id);
//             foreach (var i in postVotes.Result)
//             {
//                 if (i.VoteType == VoteType.Like)
//                 {
//                     var tmp = _postVoteService.Delete(golfer.Id, i.ID);
//                     return Ok(tmp.Result);
//                 }
//             }
//             var result = _postVoteService.Add(golfer.Id, pv);

// //         // PUT api/<CommentsController>/5
// //         [HttpPut("{id}")]
// //         async public Task<ActionResult> Put(Guid id, [FromBody] Comment comment)
// //         {
// //             comment.ID = id;
// //             var golfer = (Golfer)HttpContext.Items["Golfer"];
// //             var result = _commentService.Edit(golfer.Id, comment);
// //             if (result != null)
// //             {
// //                 return Ok(result);
// //             }
// //             else return BadRequest(result.Exception.Message);
// //         }
// //         [HttpPost("{id}/like")]
// //         async public Task<ActionResult> LikeComment(Guid id)
// //         {
// //             var golfer = (Golfer)HttpContext.Items["Golfer"];
// //             var post = _postService.Get(id);
// //             if (post == null)
// //             {
// //                 return BadRequest();
// //             }
// //             PostVote pv = new PostVote();
// //             pv.CommentID = id;
// //             pv.GolferID = golfer.Id;
// //             pv.VoteType = VoteType.Like;
// //             var postVotes = _postVoteService.GetPostVote(golfer.Id, id);
// //             foreach (var i in postVotes.Result)
// //             {
// //                 if (i.VoteType == VoteType.Like)
// //                 {
// //                     return BadRequest(pv);
// //                 }
// //             }
// //             var result = _postVoteService.Add(golfer.Id, pv);


//             if (result.IsCompletedSuccessfully)
//             {
//                 return Ok(result.Result);
//             }
//             else return BadRequest(result.Exception.Message);

// //             if (result.IsCompletedSuccessfully)
// //             {
// //                 return Ok(result);
// //             }
// //             else return BadRequest(result.Exception.Message);

//         }
//         // DELETE api/<CommentsController>/5
//         [HttpDelete("{id}")]
//         async public Task<ActionResult> Delete(Guid id)
//         {
//             var golfer = (Golfer)HttpContext.Items["Golfer"];
//             var result = _commentService.Delete(golfer.Id, id);
//             if (result != null)
//             {
//                 return Ok(result.Result);
//             }
//             else return BadRequest(result);
//         }
//     }
// }
