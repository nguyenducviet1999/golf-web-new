using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Golf.Domain.GolferData;
using Golf.EntityFrameworkCore.Repositories;
using Golf.EntityFrameworkCore;

using Golf.Domain.Post;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Post;
using Golf.Domain.Resources;
using Golf.Domain.Shared.Resources;

using Golf.Core.Exceptions;
using Golf.Core.Common.Post;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.PostController.Requests;
using Golf.Services.Notifications;

namespace Golf.Services
{
    public class CommentService
    {
        private readonly CommentRepository _commentRepository;
        private readonly NotificationService _notificationService;
        private readonly PostVoteService _postVoteService;
        private DatabaseTransaction _databaseTransaction;
        private readonly PostRepository _postRepository;
        private readonly PhotoService _photoService;
        private readonly GolferService _golferService;
        private readonly IMapper _mapper;

        public CommentService(
          NotificationService notificationService,
          DatabaseTransaction databaseTransaction,
          CommentRepository commentRepository,
          PostVoteService postVoteService,
          PostRepository postRepository,
          PhotoService photoService,
          GolferService golferService,
          IMapper mapper)
        {
            _notificationService = notificationService;
            _databaseTransaction = databaseTransaction;
            _commentRepository = commentRepository;
            _postVoteService = postVoteService;
            _postRepository = postRepository;
            _photoService = photoService;
            _golferService = golferService;
            _mapper = mapper;
        }

        public List<Comment> GetChildrenComments(Guid CommentID, int startIndex)
        {
            return _commentRepository
                .Find(comment => comment.ParentID == CommentID && comment.DeleteBy == null && comment.DeleteDate == null)
                .OrderBy(comment => comment.CreatedDate)
                .Skip(startIndex)
                .Take(10)
                .ToList();
        }

        public List<CommentDetailResponse> GetPostComments(Guid PostID, Golfer currentGolfer, int startIndex)
        {
            List<Comment> comments = _commentRepository
                        .Find(comment => comment.ParentID == null && comment.PostID == PostID && comment.DeleteBy == null && comment.DeleteDate == null)
                        .OrderByDescending(comment => comment.CreatedDate)
                        .Skip(startIndex)
                        .Take(Const.PageSize).OrderBy(comment => comment.CreatedDate).ToList();

            List<CommentDetailResponse> commentDetailResponses = new List<CommentDetailResponse>();

            for (var index = 0; index < comments.Count(); index++)
            {

                CommentDetailResponse commentDetailResponse = _mapper.Map<CommentDetailResponse>(this.GetComment(comments[index], currentGolfer));
                var child = _commentRepository
                 .Find(comment => comment.ParentID == comments[index].ID && comment.DeleteBy == null && comment.DeleteDate == null)
                 .OrderByDescending(comment => comment.CreatedDate)
                 .Skip(0)
                 .Take(3)
                 .OrderBy(comment => comment.CreatedDate)
                 .ToList();
                foreach (var i in child)
                {
                    commentDetailResponse.Child.Add(GetComment(i, currentGolfer));
                }

                commentDetailResponses.Add(commentDetailResponse);

            }

            return commentDetailResponses;
        }

        public CommentResponse GetComment(Comment comment, Golfer currentGolfer)
        {
            //int totalReplies = _postVoteService.CountCommentVoteDetail(comment.ID, VoteType.Reply);
            int totalReplies = _commentRepository.CountAll(cm => cm.ParentID == comment.ID && cm.DeleteDate == null);
            int totalLikes = _postVoteService.CountCommentVoteDetail(comment.ID, VoteType.Like);

            List<MinimizedGolfer> tagGolfers = new List<MinimizedGolfer>();
            var currentGolferLikeStatus = _postVoteService.GetCurrentGolferLikeStatusOfComment(comment.ID, currentGolfer.Id);
            List<Golfer> taggedGolfers = comment.TagIDs.Count() != 0 ? _golferService.GetGolfers(comment.TagIDs) : new List<Golfer>();

            foreach (Golfer taggedGolfer in taggedGolfers)
            {
                tagGolfers.Add(_mapper.Map<MinimizedGolfer>(taggedGolfer));
            }
            var commentResponse = new CommentResponse
            {
                Content = comment.Content,
                TagGolfers = tagGolfers,
                IsLiked = currentGolferLikeStatus,
                CreatedDate = comment.CreatedDate,
                ID = comment.ID,
                ModifiedDate = comment.ModifiedDate,
                Owner = _mapper.Map<MinimizedGolfer>(comment.Golfer),
                PhotoID = comment.PhotoNames,
                TotalReplies = totalReplies,
                TotalLikes = totalLikes,
                ParentID = comment.ParentID
            };
            return commentResponse;
        }

        public CommentDetailResponse GetCommentDetail(Guid CommentID, Golfer currentGolfer, int startIndex)
        {
            Comment comment = _commentRepository.Get(CommentID);
            int totalReplies = _commentRepository.CountAll(cmt => cmt.ParentID == CommentID); // _postVoteService.CountCommentVoteDetail(comment.ID, VoteType.Reply);
            int totalLikes = _postVoteService.CountCommentVoteDetail(comment.ID, VoteType.Like);
            List<MinimizedGolfer> tagGolfers = new List<MinimizedGolfer>();
            var currentGolferLikeStatus = _postVoteService.GetCurrentGolferLikeStatusOfPost(comment.ID, currentGolfer.Id);
            List<Golfer> taggedGolfers = comment.TagIDs.Count() != 0 ? _golferService.GetGolfers(comment.TagIDs) : new List<Golfer>();
            foreach (Golfer taggedGolfer in taggedGolfers)
            {
                tagGolfers.Add(_mapper.Map<MinimizedGolfer>(taggedGolfer));
            }
            List<Comment> childrenComments = GetChildrenComments(comment.ID, startIndex);
            List<CommentResponse> child = new List<CommentResponse>();
            for (var i = 0; i < childrenComments.Count(); i++)
            {
                child.Add(GetComment(childrenComments[i], currentGolfer));
            }
            var commentResponse = new CommentDetailResponse
            {
                Content = comment.Content,
                Child = child,
                TagGolfers = tagGolfers,
                IsLiked = currentGolferLikeStatus,
                CreatedDate = comment.CreatedDate,
                ID = comment.ID,
                ModifiedDate = comment.ModifiedDate,
                Owner = _mapper.Map<MinimizedGolfer>(comment.Golfer),
                PhotoID = comment.PhotoNames,
                TotalReplies = totalReplies,
                TotalLikes = totalLikes,
            };
            return commentResponse;
        }

        public async Task<Comment> AddComment(Guid PostID, Golfer currentGolfer, AddCommentRequest request)
        {
            Post post = _postRepository.Get(PostID);
            Photo photo = new Photo();
            if (post != null && post.DeleteBy == null && post.DeleteDate == null)
            {
                try
                {
                    _databaseTransaction.BeginTransaction();
                    Comment comment = new Comment
                    {
                        Golfer = currentGolfer,
                        ID = Guid.NewGuid(),
                        Content = request.Content,
                        CreatedBy = currentGolfer.Id,
                        CreatedDate = DateTime.Now,
                        TagIDs = request.TagIDs,
                        PostID = PostID,
                    };
                    if (request.File != null)
                    {
                         photo = await _photoService.SafeSavePhoto(currentGolfer.Id, request.File, PhotoType.Comment);
                        comment.PhotoNames = photo.Name;
                    }
                    _commentRepository.SafeAdd(comment);
                    await _databaseTransaction.Commit();
                    //notification
                    foreach (var i in comment.TagIDs)
                    {
                        _notificationService.NotificationMentionsUserComment(currentGolfer.Id, i, PostID);
                    }
                    if (post.OwnerID != currentGolfer.Id)
                    {
                        _notificationService.NotificationCommentYousPost(currentGolfer.Id, post.OwnerID, PostID);
                    }
                    foreach (var i in post.TagIDs)
                    {
                        if (i != currentGolfer.Id)
                        {
                            _notificationService.NotificationCommentYousTagPost(currentGolfer.Id, i, PostID);
                        }
                    }
                    //notification
                    return comment;
                }
                catch (Exception exception)
                {
                    _databaseTransaction.Rollback();
                    _photoService.DeletePhoto(photo.Name);
                    throw exception;
                }
            }
            throw new NotFoundException("Can't find post");
        }

        public async Task<CommentResponse> EditComment(Guid PostID, Guid CommentID, Golfer currentGolfer, EditCommentRequest request)
        {
            Post post = _postRepository.Get(PostID);
            if (post != null && post.DeleteBy == null && post.DeleteDate == null)
            {
                Comment comment = _commentRepository.Get(CommentID);
                if (comment != null && comment.DeleteBy == null && comment.CreatedBy == null)
                {
                    if (comment.CreatedBy == currentGolfer.Id)
                    {
                        try
                        {
                            _databaseTransaction.BeginTransaction();
                            if (request.File != null)
                            {
                                Photo photo = await _photoService.SafeSavePhoto(currentGolfer.Id, request.File, PhotoType.Comment);
                                comment.PhotoNames = photo.Name;
                            }
                            comment.Content = request.Content;
                            comment.TagIDs = request.TagIDs;
                            comment.ModifiedBy = currentGolfer.Id;
                            comment.ModifiedDate = DateTime.Now;
                            _commentRepository.SafeUpdate(comment);
                            await _databaseTransaction.Commit();
                        }
                        catch (Exception exception)
                        {
                            _databaseTransaction.Rollback();
                            throw exception;
                        }
                        return GetComment(comment, currentGolfer);
                    }
                    throw new ForbiddenException("Access denied");
                }
                throw new NotFoundException("Can't find comment");
            }
            throw new NotFoundException("Can't find post");
        }

        public bool RemoveComment(Guid PostID, Guid CommentID, Golfer currentGolfer)
        {
            Post post = _postRepository.Get(PostID);
            if (post != null && post.DeleteBy == null && post.DeleteDate == null)
            {
                Comment comment = _commentRepository.Get(CommentID);
                if (comment != null && comment.DeleteBy == null)
                {
                    if (comment.CreatedBy == currentGolfer.Id || post.OwnerID == currentGolfer.Id)
                    {
                        comment.DeleteBy = currentGolfer.Id;
                        comment.DeleteDate = DateTime.Now;
                        _commentRepository.UpdateEntity(comment);
                        return true;
                    }
                    throw new ForbiddenException("Access denied");
                }
                throw new NotFoundException("Can't find comment");
            }
            throw new NotFoundException("Can't find post");
        }

        public async Task<CommentDetailResponse> ReplyComment(Guid PostID, Guid ParentCommentID, Golfer currentGolfer, AddCommentRequest request)
        {
            Post post = _postRepository.Get(PostID);
            if (post != null && post.DeleteBy == null && post.DeleteDate == null)
            {
                Comment parentComment = _commentRepository.Get(ParentCommentID);
                if (parentComment != null && parentComment.DeleteBy == null && parentComment.DeleteDate == null)
                {
                    try
                    {
                        _databaseTransaction.BeginTransaction();
                        Comment comment = new Comment
                        {
                            ID = Guid.NewGuid(),
                            Golfer = currentGolfer,
                            Content = request.Content,
                            CreatedBy = currentGolfer.Id,
                            CreatedDate = DateTime.Now,
                            TagIDs = request.TagIDs,
                            PostID = PostID,
                            ParentID = ParentCommentID
                        };
                        if (request.File != null)
                        {
                            Photo photo = await _photoService.SafeSavePhoto(currentGolfer.Id, request.File, PhotoType.Comment);
                            comment.PhotoNames = photo.Name;
                        }
                        _commentRepository.SafeAdd(comment);
                        await _databaseTransaction.Commit();
                        //make notification
                        foreach (var i in comment.TagIDs)
                        {
                            _notificationService.NotificationMentionsUserComment(currentGolfer.Id, i, PostID);
                        }
                        if ((Guid)parentComment.CreatedBy != currentGolfer.Id)
                        {
                            _notificationService.NotificationRepliedYourComment(currentGolfer.Id, (Guid)parentComment.CreatedBy, PostID);
                            if (post.OwnerID != currentGolfer.Id && post.OwnerID != (Guid)parentComment.CreatedBy)
                            {
                                _notificationService.NotificationCommentYousPost(currentGolfer.Id, post.OwnerID, PostID);
                            }
                        }
                        foreach (var i in post.TagIDs)
                        {
                            if ((Guid)parentComment.CreatedBy != currentGolfer.Id && i != currentGolfer.Id && i != (Guid)parentComment.CreatedBy)
                            {
                                _notificationService.NotificationCommentYousTagPost(currentGolfer.Id, i, PostID);
                            }
                        }
                        //make notification 
                    }
                    catch (Exception exception)
                    {
                        _databaseTransaction.Rollback();
                        throw exception;
                    }
                    int totalCommentsReply = _commentRepository.CountAll(comment => comment.ParentID == ParentCommentID && comment.DeleteBy == null && comment.DeleteDate == null);
                    return GetCommentDetail(parentComment.ID, currentGolfer, Const.PageSize * (int)(totalCommentsReply / Const.PageSize));
                }
                throw new NotFoundException("Can't find comment");
            }
            throw new NotFoundException("Can't find post");
        }

        public List<MinimizedGolfer> GetDetailVoteTypeOfComment(Guid PostID, Guid CommentID, GetDetailCommentVoteTypeRequest request, int startIndex)
        {
            VoteType type;
            switch (request)
            {
                case GetDetailCommentVoteTypeRequest.Like:
                    type = VoteType.Like;
                    break;
                case GetDetailCommentVoteTypeRequest.Reply:
                    type = VoteType.Reply;
                    break;
                default:
                    throw new BadRequestException("Undefined vote type");
            }
            List<PostVote> postVotes = _postVoteService.GetDetailVoteTypeOfComment(PostID, CommentID, type, startIndex);
            List<Guid> golferIDs = postVotes.Select(pv => pv.CreatedBy.GetValueOrDefault()).ToList();
            List<Golfer> golfers = _golferService.GetGolfers(golferIDs);
            return golfers.Select(golfer => _mapper.Map<MinimizedGolfer>(golfer)).ToList();
        }
    }
}
