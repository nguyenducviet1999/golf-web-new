using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Core.Common.Post;
using Golf.Domain.Shared;
using Golf.Domain.SocialNetwork;
using Golf.Domain.Shared.Post;
using Golf.Domain.Post;

namespace Golf.Services
{
    public class NewsfeedService
    {
        private readonly RelationshipService _relationshipService;
        private readonly PostRepository _postRepository;
        private readonly GroupService _groupService;
        private readonly PostService _postService;

        public NewsfeedService(PostService postService,
            RelationshipService relationshipService,
            GroupService groupService,
            PostRepository postRepository)
        {
            _relationshipService = relationshipService;
            _postRepository = postRepository;
            _groupService = groupService;
            _postService = postService;
        }

        /// <summary>
        /// L?y d? li?u trang new feed ng??i d�ng
        /// </summary>
        /// <param name="currentGolferID">??nh danh ng??i d�ng hi?n t?i</param>
        /// <param name="startIndex">V? tr� l?y ??u ti�n</param>
        /// <returns>Danh s�ch d? li?u b�i vi?t</returns>
        public async Task<List<PostResponse>> GetGolferNewsfeed(Guid currentGolferID, int startIndex)
        {
            List<Guid> groupIDs = _groupService.GetGroupIDsOfGolfer(currentGolferID);
            List<Guid> relationshipIDs = _relationshipService.GetGolferRelationshipIDs(currentGolferID);
            var posts = _postRepository.Find(p => (p.OwnerID == currentGolferID || p.Privacy == PostPrivacyLevel.Public || (relationshipIDs.Contains(p.OwnerID) && p.Privacy != PostPrivacyLevel.Private))&&p.GroupID==null && p.DeleteBy == null && p.DeleteDate == null)
                    .OrderByDescending(p => p.CreatedDate)
                    .Skip(startIndex)
                    .Take(Const.PageSize)
                    .ToList();
            List<PostResponse> postResponses = new List<PostResponse>();
            foreach (Post post in posts)
            {
                var postResponse = _postService.GetPostResponse(post, currentGolferID).Result;
                if (postResponse != null)
                {
                    postResponses.Add(postResponse);
                }
            }
            return postResponses;
        }

        /// <summary>
        /// L?y c�c b�i vi?t tr�n trang c� nh�n c?a m?t ng??i n�o ?�
        /// </summary>
        /// <param name="GolferID">??nh danh ng??i d�ng</param>
        /// <param name="currentGolferID">Ng??i d�ng hi?n t?i</param>
        /// <param name="startIndex">v? tr� l?y(Ph�n trang)</param>
        /// <returns></returns>
        public async Task<List<PostResponse>> GetGolferPosts(Guid GolferID, Guid currentGolferID, int startIndex)
        {
            bool relationship = _relationshipService.GetRelationshipWithGolfer(currentGolferID, GolferID);
            var posts = _postRepository.Find(p => p.GroupID == null && p.CreatedBy == GolferID && p.DeleteBy == null && p.DeleteDate == null)
            //&& (relationship == true ? p.Privacy == PostPrivacyLevel.Friends || p.Privacy == PostPrivacyLevel.Public : p.Privacy == PostPrivacyLevel.Public))
            .OrderByDescending(p => p.CreatedDate)
            .Skip(startIndex)
            .Take(Const.PageSize)
            .ToList();
            List<PostResponse> postResponses = new List<PostResponse>();

            foreach (Post post in posts)
            {
                var postResponse = _postService.GetPostResponse(post, currentGolferID).Result;
                if (postResponse != null)
                {
                    postResponses.Add(postResponse);
                }
            }
            return postResponses;
        }

        /// <summary>
        /// L?y b�i vi?t trong nh�m
        /// </summary>
        /// <param name="currentGolferID">??nh danh ng??i d�ng hi?n th?i</param>
        /// <param name="GroupID">??nh danh nh�m</param>
        /// <param name="startIndex">v? tr� ph�n trang</param>
        /// <returns></returns>
        public async Task<List<PostResponse>> GetGroupPosts(Guid currentGolferID, Guid GroupID, int startIndex)
        {
            var posts = _postRepository.Find(p => p.GroupID != null && p.DeleteBy == null && p.DeleteDate == null && p.Privacy == PostPrivacyLevel.Group)
                .OrderByDescending(p => p.CreatedDate)
                .Skip(startIndex)
                .Take(Const.PageSize)
                .ToList();
            List<PostResponse> postResponses = new List<PostResponse>();

            foreach (Post post in posts)
            {
                var postResponse = _postService.GetPostResponse(post, currentGolferID).Result;
                if (postResponse != null)
                {
                    postResponses.Add(postResponse);
                }
            }
            return postResponses;
        }
    }
}