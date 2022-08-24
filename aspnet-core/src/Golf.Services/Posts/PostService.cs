using AutoMapper;

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Golf.EntityFrameworkCore.Repositories;
using Golf.EntityFrameworkCore;

using Golf.Domain.Post;
using Golf.Domain.GolferData;
using Golf.Domain.Resources;
using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Post;
using Golf.Domain.Shared.Resources;

using Golf.Core.Exceptions;
using Golf.Core.Common.Post;
using Golf.Core.Common.Scorecard;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.ScorecardController.Requests;
using Golf.Core.Dtos.Controllers.PostController.Requests;
using Golf.Domain.Shared.Scorecard;
using Golf.Services.Notifications;
using Golf.Services.Events;
using Golf.Services.Tournaments;
using Golf.Domain.SocialNetwork;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Domain.Shared;

namespace Golf.Services
{
    public class PostService
    {
        private readonly RelationshipService _relationshipService;
        private readonly CommentRepository _commentRepository;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly PostVoteService _postVoteService;
        private readonly ScorecardService _scorecardService;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly PostRepository _postRepository;
        private readonly PhotoService _photoService;
        private readonly GolferService _golferService;
        private readonly NotificationService _notificationService;
        private readonly EventService _eventService;
        private readonly TournamentService _tournamentService;
        private readonly EventRepository _eventRepository;
        private readonly TournamentRepository _tournamentRepository;
        private readonly IMapper _mapper;
        private readonly ScorecardVoteRepository _scorecardVoteRepository;
        public PostService(
            GroupMemberRepository groupMemberRepository,
            ScorecardVoteRepository scorecardVoteRepository,
            TournamentRepository tournamentRepository,
            EventRepository eventRepository,
            TournamentService tournamentService,
            EventService eventService,
            NotificationService notificationService,
            ScorecardRepository scorecardRepository,
            RelationshipService relationshipService,
            DatabaseTransaction databaseTransaction,
            CommentRepository commentRepository,
            ScorecardService scorecardService,
            PostVoteService postVoteService,
            GroupRepository groupRepository,
            PostRepository postRepository,
            PhotoService photoService,
            GolferService golferService,
            IMapper mapper)
        {
            _groupMemberRepository = groupMemberRepository;
            _scorecardVoteRepository = scorecardVoteRepository;
            _tournamentRepository = tournamentRepository;
            _eventRepository = eventRepository;
            _tournamentService = tournamentService;
            _eventService = eventService;
            _scorecardRepository = scorecardRepository;
            _notificationService = notificationService;
            _databaseTransaction = databaseTransaction;
            _relationshipService = relationshipService;
            _commentRepository = commentRepository;
            _scorecardService = scorecardService;
            _postVoteService = postVoteService;
            _groupRepository = groupRepository;
            _postRepository = postRepository;
            _photoService = photoService;
            _golferService = golferService;
            _mapper = mapper;
        }

        public async Task<PostResponse> GetPostResponse(Post post, Guid currentGolferID)
        {
            if (post.DeleteDate != null && post.DeleteBy != null)
            {
                return null;
            }

            List<Golfer> taggedGolfers = post.TagIDs.Count() != 0 ? _golferService.GetGolfers(post.TagIDs) : new List<Golfer>();
            List<MinimizedGolfer> tagGolfers = taggedGolfers.Select(golfer => _mapper.Map<MinimizedGolfer>(golfer)).ToList();
            var currentGolferLikeStatus = _postVoteService.GetCurrentGolferLikeStatusOfPost(post.ID, currentGolferID);
            var postVotesStatus = _postVoteService.GetVotesOfPost(post.ID, currentGolferID);

            var postResponse = new PostResponse
            {
                ID = post.ID,
                TotalComments = postVotesStatus.TotalComments,
                TotalLikes = postVotesStatus.TotalLikes,
                TotalConfirms = postVotesStatus.TotalConfirms,
                TotalIncorrects = postVotesStatus.TotalIncorrects,
                TotalShares = postVotesStatus.TotalShares,
                Owner = _mapper.Map<MinimizedGolfer>(post.Owner),
                ModifiedDate = post.ModifiedDate,
                CreatedDate = post.CreatedDate,
                Privacy = post.Privacy,
                Photos = post.GetPhotoNames(),
                IsLiked = currentGolferLikeStatus,
                TagGolfers = tagGolfers,
                Content = post.Content,
                PostAction = post.PostAction,
                ParentID=post.ParentID
            };

            if (post.ReferenceObject != null && post.ReferenceObject.Count() != 0)
            {

                foreach (var i in post.ReferenceObject)
                {
                    if (i.Type == ReferenceObjectType.Event)
                    {
                        var ev = _eventRepository.Find(e => e.ID == i.ID).FirstOrDefault();
                        if (ev != null)
                        {
                            var evt = _eventService.GetEventDetail(currentGolferID, ev);
                            if (evt == null)
                                return null;
                            postResponse.Events.Add(evt);
                        }
                        else
                        {
                            _postRepository.RemoveEntity(post);
                            return null;
                        }
                    }
                    if (i.Type == ReferenceObjectType.Scorecard)
                    {
                        var sc = _scorecardRepository.FindScorecard(sc => sc.ID == i.ID).FirstOrDefault();
                        if (sc != null)
                        {
                            postResponse.Scorecards.Add(_scorecardService.GetMinimizedScorecard(sc));
                            if (post.OwnerID == sc.OwnerID)
                            {
                                var scorecardVote = _scorecardVoteRepository.Find(scv => scv.ScorecardID == sc.ID).OrderBy(scv => scv.CreatedDate).FirstOrDefault();
                                if (scorecardVote != null)
                                {
                                    postResponse.ConfirmGolfer = _golferService.GetMinimizedGolfer(scorecardVote.GolferID);
                                }
                                var curentScorecardVote = _scorecardVoteRepository.Find(scv => scv.ScorecardID == sc.ID && scv.GolferID == currentGolferID).OrderBy(scv => scv.CreatedDate).FirstOrDefault();
                                if (curentScorecardVote != null)
                                {
                                    postResponse.ConfirmType = curentScorecardVote.Type;
                                }
                            }
                        }
                        else
                        {
                            _postRepository.RemoveEntity(post);
                            return null;
                        }
                    }
                    if (i.Type == ReferenceObjectType.Tournament)
                    {
                        var t = _tournamentRepository.Find(t => t.ID == i.ID).FirstOrDefault();
                        if (t != null)
                        {
                            postResponse.Tournaments.Add(_tournamentService.GetTournammentRespone(currentGolferID, t));
                        }
                        else
                        {
                            _postRepository.RemoveEntity(post);
                            return null;
                        }
                    }

                }
            }
            if (post.GroupID != Guid.Empty && post.GroupID != null && post.Group != null)
            {
                postResponse.GroupID = post.GroupID;
                postResponse.GroupName = post.Group.Name;
            }
            if (post.ParentID != null)
            {
                postResponse.Parent = await GetPostResponse(_postRepository.GetPost((Guid)post.ParentID), currentGolferID);
            }

            return postResponse;
        }

        public async Task<PostResponse> Get(Guid PostID, Golfer currentGolfer)
        {
            Post post = _postRepository.GetPost(PostID);
            return await GetPostResponse(post, currentGolfer.Id);
        }

        public async Task<PostResponse> Add(Golfer currentGolfer, CreatePostRequest request)
        {
            Guid PostID = Guid.NewGuid();
            try
            {
                _databaseTransaction.BeginTransaction();
                List<Photo> photos = new List<Photo>();
                //PostType type = PostType.Default;
                List<ReferenceObject> ReferenceObjects = new List<ReferenceObject>();
                if (request.ScorecardIDs != null)
                {
                    foreach (var i in request.ScorecardIDs)
                    {
                        var sc = _scorecardRepository.Get(i);
                        if (sc == null)
                        {
                            throw new NotFoundException("Not found Scorecard");
                        }
                        if(sc.Type!=ScorecardType.Pending||sc.OwnerID!=currentGolfer.Id)
                        {
                            throw new BadRequestException("Access Denied!");
                        }
                        sc.Type = ScorecardType.Posted;
                        _scorecardRepository.SafeUpdate(sc);
                        ReferenceObjects.Add(new ReferenceObject() { ID = i, Type = ReferenceObjectType.Scorecard });
                    }
                }
                if (request.EventIDs != null)
                {
                    foreach (var i in request.EventIDs)
                    {
                        var ev = _eventRepository.Get(i);
                        if (ev == null)
                        {
                            throw new NotFoundException("Not found event");
                        }
                        ReferenceObjects.Add(new ReferenceObject() { ID = i, Type = ReferenceObjectType.Event });
                    }
                }
                if (request.TournamentIDs != null)
                {
                    foreach (var i in request.TournamentIDs)
                    {
                        var t = _tournamentRepository.Get(i);
                        if (t == null)
                        {
                            throw new NotFoundException("Not found Tournament");
                        }
                        ReferenceObjects.Add(new ReferenceObject() { ID = i, Type = ReferenceObjectType.Tournament });
                    }
                }
                Post post = new Post
                {
                    ID = PostID,
                    //Type = type,
                    Privacy = request.Privacy,
                    Content = request.Content,
                    // PhotoNames = request.Photos,// photos.Select(photo => photo.ID).ToList(),
                    ReferenceObject = ReferenceObjects,
                    TagIDs = request.TagIDs,
                    CreatedBy = currentGolfer.Id,
                    CreatedDate = DateTime.Now,
                    Owner = currentGolfer,
                    PostAction = request.PostFeeling,
                };
                post.SetPhotoNames(request.Photos);
                if (request.GroupID != Guid.Empty)
                {
                    var group = _groupRepository.Get(request.GroupID);
                    post.Group = group;
                }
                _postRepository.SafeAdd(post);
                await _databaseTransaction.Commit();
                //make notification
                if (post.GroupID! != null)
                {
                    await NotificationPostGroup(currentGolfer.Id, (Guid)post.GroupID, post);
                }
                if (post.TagIDs.Count > 0)
                {
                    foreach (var i in post.TagIDs)
                    {
                        _notificationService.NotificationTagUserInPost(currentGolfer.Id, i, post.ID);
                    }
                }
                //make notification
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw exception;
            }
            return await GetPostResponse(_postRepository.Get(PostID), currentGolfer.Id);
        }

        public async Task<PostResponse> Edit(Guid PostID, Golfer currentGolfer, EditPostRequest request)
        {
            Post post = _postRepository.Get(PostID);
            var scorecard = _scorecardRepository.Get(post.ScorecardIDs().FirstOrDefault());
            if (post.ScorecardIDs().Count() > 0)
            {
                foreach (var i in request.TagIDs)
                {
                    _notificationService.NotificationPlayWithFriend(currentGolfer.Id, i, PostID, scorecard.CourseID);
                }
            }
            else
            {
                foreach (var i in request.TagIDs)
                {
                    if (post.TagIDs.Contains(i) == false)
                    {
                        _notificationService.NotificationTagUserInPost(currentGolfer.Id, i, PostID);
                    }
                }
            }
            if (post != null && post.DeleteBy == null && post.DeleteDate == null)
            {
                if ((Guid)post.CreatedBy == currentGolfer.Id)
                {
                    try
                    {
                        _databaseTransaction.BeginTransaction();
                        var tmp = post.GetPhotoNames();
                        var deletePhotos = tmp.RemoveAll(pt => request.ClientCurrentPhotoIDs.Contains(pt));
                        _photoService.DeletePhotos(currentGolfer.Id, tmp);
                        /// Example: ClientCurrentPhotoID photo name only
                        //List<string> deletePhotoIDs = request.ClientCurrentPhotoIDs.Where(photoID => post.PhotoIDs.IndexOf(photoID) == -1).ToList();
                        //_photoService.DeletePhotos(currentGolfer.Id, deletePhotoIDs);
                        // List<Photo> photos = await _photoService.SavePhotos(currentGolfer.Id, request.Files, PhotoType.Post);
                        List<string> photoIDs = request.photos; //photos.Select(photo => photo.ID).ToList();
                        photoIDs.AddRange(request.ClientCurrentPhotoIDs);
                        post.TagIDs = request.TagIDs;
                        post.SetPhotoNames(photoIDs);
                        post.Privacy = request.Privacy;
                        post.Content = request.Content;
                        post.ModifiedBy = currentGolfer.Id;
                        post.ModifiedDate = DateTime.Now;
                        _postRepository.SafeUpdate(post);
                        await _databaseTransaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        _databaseTransaction.Rollback();
                        throw exception;
                    }
                    return await GetPostResponse(post, currentGolfer.Id);
                }
                throw new ForbiddenException("Access denied");
            }
            throw new NotFoundException("Can't find post");
        }

        public async Task<bool> Remove(Guid PostID, Golfer currentGolfer)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                Post post = _postRepository.Get(PostID);
                if (post != null && post.DeleteBy == null && post.DeleteDate == null)
                {
                    if ((Guid)post.CreatedBy == currentGolfer.Id)
                    {
                        post.DeleteBy = currentGolfer.Id;
                        post.DeleteDate = DateTime.Now;
                        _photoService.SafeDeletePhotoDatas(currentGolfer.Id, post.GetPhotoNames());
                        _postRepository.SafeUpdate(post);
                        await _databaseTransaction.Commit();
                        return true;
                    }
                    throw new ForbiddenException("Access denied");
                }
                throw new NotFoundException("Can't find post");
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        public async Task<PostResponse> Share(Guid PostID, Golfer currentGolfer, SharePostRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                Post sharePost = _postRepository.Get(PostID);
                if (sharePost != null && sharePost.DeleteBy == null && sharePost.DeleteDate == null)
                {
                    if (sharePost.Privacy != PostPrivacyLevel.Public)
                    {
                        throw new BadRequestException("Can't share post");
                    }
                    Post post = new Post
                    {
                        //Type = PostType.Default,
                        Privacy = request.Privacy,
                        Content = request.Content,
                        TagIDs = request.TagIDs,
                        Parent = sharePost.Parent == null ? sharePost : sharePost.Parent,
                        CreatedBy = currentGolfer.Id,
                        CreatedDate = DateTime.Now,
                        Owner = currentGolfer,
                        //PostAction = request.PostFeeling
                    };
                    if (request.GroupID != Guid.Empty)
                    {
                        var group = _groupRepository.Get(request.GroupID);
                        post.Group = group;
                    }
                    _postRepository.SafeAdd(post);
                    await _databaseTransaction.Commit();
                    if (sharePost.OwnerID != currentGolfer.Id)
                    {
                        _notificationService.NotificationSharePost(currentGolfer.Id, sharePost.OwnerID, PostID);
                    }
                    if (post.GroupID! != null)
                    {
                        await NotificationPostGroup(currentGolfer.Id, (Guid)post.GroupID, post);
                    }
                    //_postVoteService.AddPostVote(post.ID, currentGolfer.Id, VoteType.Share);
                    //_postRepository.Add(post);
                    return await GetPostResponse(post, currentGolfer.Id);
                }
                throw new NotFoundException("Can't find post");
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Lấy danh sách người dùng vote Post
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="request"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public List<MinimizedGolfer> GetDetailVoteTypeOfPost(Guid PostID, GetDetailPostVoteTypeRequest request, int startIndex)
        {
            Post post = _postRepository.Get(PostID);
            if (post == null)
            {
                throw new NotFoundException("Not found post");
            }
            Scorecard scorecard = null;
            ScorecardVote scorecardVote = null;
            foreach (var i in post.ScorecardIDs())
            {
                var sc = _scorecardRepository.Get(i);
                if (sc == null)
                    continue;
                if (sc.OwnerID == post.OwnerID)
                {
                    scorecard = sc;
                    break;
                }
            }
            VoteType type;
            switch (request)
            {
                case GetDetailPostVoteTypeRequest.Like:
                    type = VoteType.Like;
                    List<PostVote> postVotes = _postVoteService.GetDetailVoteTypeOfPost(PostID, type, startIndex);
                    List<Guid> golferIDs = postVotes.Select(pv => pv.CreatedBy.GetValueOrDefault()).ToList();
                    List<Golfer> golfers = _golferService.GetGolfers(golferIDs);
                    return golfers.Select(golfer => _mapper.Map<MinimizedGolfer>(golfer)).ToList();
                case GetDetailPostVoteTypeRequest.Comment:
                    var VoterIDs = _commentRepository.Find(cm => cm.PostID == PostID).GroupBy(cm => cm.Golfer).Select(cm=>cm.Key.Id).Skip(startIndex).Take(Const.PageSize).ToList();
                    return _golferService.GetMinimizedGolfers(VoterIDs);
                case GetDetailPostVoteTypeRequest.Share:
                    var sharerIDs=_postRepository.Find(p => p.ParentID == PostID).GroupBy(p=>p.Owner).Select(cm => cm.Key.Id).Skip(startIndex).Take(Const.PageSize).ToList();
                    return _golferService.GetMinimizedGolfers(sharerIDs);
                case GetDetailPostVoteTypeRequest.Confirm:
                    if(scorecard==null)
                    {
                        return new List<MinimizedGolfer>();
                    }    
                    var confirmerIDs=_scorecardVoteRepository.Find(sv=>sv.ScorecardID==scorecard.ID&&sv.Type==ScorecardVoteType.Confirm).GroupBy(sv=>sv.Golfer).Select(sv=>sv.Key.Id).Skip(startIndex).Take(Const.PageSize).ToList();
                    return _golferService.GetMinimizedGolfers(confirmerIDs);
                case GetDetailPostVoteTypeRequest.ConfirmIncorrect:
                    if (scorecard == null)
                    {
                        return new List<MinimizedGolfer>();
                    }
                    var confirmerIncorrectIDs = _scorecardVoteRepository.Find(sv => sv.ScorecardID == scorecard.ID && sv.Type == ScorecardVoteType.ConfirmIncorrect).GroupBy(sv => sv.Golfer).Select(sv => sv.Key.Id).Skip(startIndex).Take(Const.PageSize).ToList();
                    return _golferService.GetMinimizedGolfers(confirmerIncorrectIDs);
                default:
                    throw new BadRequestException("Undefined vote type");
            }
        }

        public async Task<PostResponse> SaveScorecards(Golfer currentGolfer, SaveScorecardsRequest request)
        {
            Guid PostID = Guid.NewGuid();
            try
            {
                _databaseTransaction.BeginTransaction();
                List<Scorecard> scorecards = new List<Scorecard>();
                foreach (SaveScorecardRequest saveScorecardRequest in request.Scorecards)
                {
                    //saveScorecardRequest.Type = ScorecardType.Posted;
                    scorecards.Add(_scorecardService.SaveScorecard(currentGolfer, saveScorecardRequest));
                    //make notification
                    if (currentGolfer.Id != saveScorecardRequest.OwnerID)
                    {
                        _notificationService.NotificationEnterYourScorecard(currentGolfer.Id, saveScorecardRequest.OwnerID, PostID);
                    }
                }
                Post post = new Post
                {
                    ID = PostID,
                    //ScorecardIDs = scorecards.Select(scorecard => scorecard.ID).ToList(),
                    //Type = PostType.Scorecard,
                    Privacy = request.Privacy,
                    Content = request.Content,
                    TagIDs = request.TagIDs,
                    CreatedBy = currentGolfer.Id,
                    CreatedDate = DateTime.Now,
                    Owner = currentGolfer,
                    //PhotoNames = request.Photos
                };
                post.SetPhotoNames(request.Photos);
                post.SetScorecardIDs(scorecards.Select(scorecard => scorecard.ID).ToList());
                if (request.GroupID != Guid.Empty)
                {
                    var group = _groupRepository.Get(request.GroupID);
                    post.Group = group;
                }
                _postRepository.SafeAdd(post);
                await _databaseTransaction.Commit();
                if (post.TagIDs.Count > 0)
                {
                    foreach (var i in post.TagIDs)
                    {
                        _notificationService.NotificationTagUserInPost(currentGolfer.Id, i, post.ID);
                    }
                }
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw exception;
            }
            return await GetPostResponse(_postRepository.GetPost(PostID), currentGolfer.Id);
        }
        public bool VoteScorecard(Guid currentGolferID, Guid postID, ScorecardVoteType scorecardVoteType)
        {
            var post = _postRepository.Get(postID);
            if (post.ScorecardIDs().Count() == 0)
            {
                throw new BadRequestException("Scorecard isn't exit");
            }
            // _postVoteService.AddPostVote(PostID, currentGolfer.Id, VoteType.Confirm);
            foreach (var i in post.ScorecardIDs())
            {
                var tmp = _scorecardRepository.Get(i);
                if (tmp == null)
                    continue;
                if (tmp.OwnerID == post.OwnerID)
                {
                    ScorecardVote scorecardVote = new ScorecardVote();
                    scorecardVote.GolferID = currentGolferID;
                    scorecardVote.ScorecardID = i;
                    scorecardVote.Type = scorecardVoteType;
                    _scorecardVoteRepository.Add(scorecardVote);
                }
                //_scorecardService.ConfirmScorecard(i, currentGolfer);
            }
            return true;
        }
        public void InviteConfirmScorecard(Guid curentGolferID, Guid postID, List<Guid> friendIDs)
        {
            var post = _postRepository.Get(postID);
            if (post == null || curentGolferID != post.OwnerID)
                return;
            foreach (var i in friendIDs)
            {
                if (_relationshipService.IsFriend(curentGolferID, i))
                {
                    _notificationService.NotificationInviteFriendConfirmScorecard(curentGolferID, i, postID);
                }
            }
        }
//group
       

        /// <summary>
        /// Thông báo bài viết nhóm
        /// </summary>
        /// <param name="currentID"></param>
        /// <param name="groupID"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public async Task NotificationPostGroup(Guid currentID,Guid groupID,Post post)
        {
            var groupMembers= _groupMemberRepository.FindWithGolfwer(gm => gm.GroupID == groupID);
            foreach(var i in groupMembers)
            {
                var setting= _golferService.SafeMyGroupSetting(i.Golfer, groupID);
                if(setting.NotificationGroupSetting==NotificationGroupSetting.All||(setting.NotificationGroupSetting == NotificationGroupSetting.Friend&& _relationshipService.IsFriend(currentID,i.GolferID)))
                {
                    _notificationService.NotificationGroupMemberPostNews(currentID, i.GolferID, groupID);
                }
            }    
        }
    }
}
