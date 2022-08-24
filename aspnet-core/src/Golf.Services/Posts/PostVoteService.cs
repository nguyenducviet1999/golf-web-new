using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Golf.Core.Exceptions;
using Golf.Domain.Post;
using Golf.Core.Common.Post;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Post;
using Golf.Services.Notifications;
using Golf.Domain.Shared.Notifications;
using Golf.Domain.Shared;
using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Scorecard;
using Golf.Domain.GolferData;

namespace Golf.Services
{
    public class PostVoteService
    {
        private readonly PostVoteRepository _postVoteRepository;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly ReportRepository _reportRepository;
        private readonly ScorecardService _scorecardService;
        private readonly CommentRepository _commentRepository;
        private readonly PostRepository _postRepository;
        private readonly GolferRepository _golferRepository;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;
        private readonly ScorecardVoteRepository _scorecardVoteRepository;

        public PostVoteService( ReportRepository reportRepository,GolferRepository golferRepository,ScorecardService scorecardService,ScorecardVoteRepository scorecardVoteRepository, CommentRepository commentRepository, ScorecardRepository scorecardRepository, PostRepository postRepository, NotificationService notificationService, PostVoteRepository postVoteRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _golferRepository = golferRepository;
            _scorecardService = scorecardService;
            _scorecardVoteRepository = scorecardVoteRepository;
            _postVoteRepository = postVoteRepository;
            _scorecardRepository = scorecardRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }
        /// <summary>
        /// Lấy số lượng vote của bài đăng
        /// </summary>
        /// <param name="PostID">Định danh bài đăng</param>
        /// <param name="curentID">Định danh người dùng</param>
        /// <returns></returns>
        public VotesOfPostResponse GetVotesOfPost(Guid PostID, Guid curentID)
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
            VotesOfPostResponse votes = new VotesOfPostResponse
            {
                IsLiked = this.GetCurrentGolferLikeStatusOfPost(PostID, curentID),
                TotalComments = _commentRepository.CountAll(cm => cm.PostID == PostID && cm.DeleteDate == null), //_postVoteRepository.CountAll(pv => pv.PostID == PostID && (pv.Type == VoteType.Comment || pv.Type == VoteType.Reply)),
                TotalIncorrects =scorecard==null?0:_scorecardVoteRepository.CountAll(pv => pv.ScorecardID == scorecard.ID && pv.Type == ScorecardVoteType.ConfirmIncorrect),
                TotalConfirms = scorecard == null ? 0 : _scorecardVoteRepository.CountAll(pv => pv.ScorecardID == scorecard.ID && pv.Type == ScorecardVoteType.Confirm),
                TotalLikes = _postVoteRepository.CountAll(pv => pv.PostID == PostID && pv.Type == VoteType.Like && pv.CommentID == null),
                TotalShares = _postRepository.CountAll(p => p.ParentID == PostID),
            };

            return votes;
        }

        public List<PostVote> GetDetailVoteTypeOfPost(Guid PostID, VoteType type, int startIndex)
        {
            return _postVoteRepository.Find(pv => pv.PostID == PostID && pv.Type == type).Skip(startIndex).Take(Const.PageSize).ToList();
        }

        public List<PostVote> GetDetailVoteTypeOfComment(Guid PostID, Guid CommentID, VoteType type, int startIndex)
        {
            return _postVoteRepository.Find(pv => pv.CommentID == CommentID && pv.Type == type && pv.PostID == PostID).Skip(startIndex).Take(Const.PageSize).ToList();
        }
        /// <summary>
        /// like, comment trong trong comment 
        /// </summary>
        /// <param name="PostID"></param>
        /// <param name="CommentID"></param>
        /// <param name="GolferID"></param>
        /// <param name="type"></param>
        public void AddCommentVote(Guid PostID, Guid CommentID, Guid GolferID, VoteType type)
        {
            PostVote postVote = _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.CommentID == CommentID && pv.PostID == PostID && pv.Type == type).FirstOrDefault();
            if ((type == VoteType.Like || type == VoteType.Confirm || type == VoteType.ConfirmIncorrect) && postVote != null)
            {
                return;
            }

            _postVoteRepository.Add(new PostVote
            {
                PostID = PostID,
                CommentID = CommentID,
                Type = type
            });
            //make notification
            switch (type)
            {
                case VoteType.Like:
                    {
                        var tmp = _commentRepository.Get(CommentID);
                        if (tmp.CreatedBy != GolferID)
                            _notificationService.NotificationLikeYourComment(GolferID, (Guid)tmp.CreatedBy, PostID);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

        }
        /// <summary>
        /// Tạo vote cho bài viết
        /// </summary>
        /// <param name="PostID">Định danh bài đăng</param>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <param name="type">Loại vote</param>
        public bool AddPostVote(Guid PostID, Guid GolferID, VoteType type)
        {
            PostVote postVote = _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.PostID == PostID && pv.CommentID == null && pv.Type == type).FirstOrDefault();
            Post post = _postRepository.Get(PostID);
            Golfer golfer = _golferRepository.Get(GolferID);
            if(post==null)
            {
                throw new NotFoundException("Not found post");
            }    
            Scorecard scorecard = null;
            ScorecardVote scorecardVote = null;
            foreach (var i in post.ScorecardIDs())
            {
                var sc = _scorecardRepository.Get(i);
                if (sc.OwnerID == post.OwnerID)
                {
                    scorecard = sc;
                    break;
                }
            }
            if (scorecard == null)
            {
                scorecardVote = null;
            }
            else
            {
                scorecardVote = _scorecardVoteRepository.Find(scv => scv.GolferID == GolferID && scv.ScorecardID == scorecard.ID).FirstOrDefault();
            }

            switch (type)
            {
                case VoteType.Confirm:
                    {
                        if (scorecard != null)
                        {
                            if (scorecard.OwnerID == GolferID)
                            {
                                return false;
                            }
                            if (scorecardVote == null)
                            {
                                _scorecardService.ConfirmScorecard(scorecard.ID,golfer);
                                _scorecardVoteRepository.Add(new ScorecardVote() { GolferID = GolferID, ScorecardID = scorecard.ID, Type = ScorecardVoteType.Confirm });
                                _notificationService.NotificationConfirmScorecard(GolferID, scorecard.OwnerID, PostID);
                            }
                            else
                            {
                                _scorecardVoteRepository.Remove(scorecardVote);
                            }
                        }
                        break;
                    }
                
                case VoteType.Like:
                    {
                        if (postVote == null)
                        {
                            _postVoteRepository.Add(new PostVote
                            {
                                PostID = PostID,
                                Type = VoteType.Like,
                            });
                            if (post.OwnerID != GolferID)
                                _notificationService.NotificationLikeYourPost(GolferID, post.OwnerID, PostID);
                            foreach (var i in post.TagIDs)
                            {
                                if (i != GolferID)
                                {
                                    _notificationService.NotificationLikeYourTagPost(GolferID, i, PostID);

                                }
                            }
                        }
                        else
                        {
                            _postVoteRepository.Remove(postVote);
                        }
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }
            return true;
        }

        public bool LikePost(Guid PostID, Guid GolferID)
        {
            PostVote postVote = _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.PostID == PostID && pv.CommentID == null && pv.Type == VoteType.Like).FirstOrDefault();
            if (postVote != null)
            {
                _postVoteRepository.Remove(postVote);
                return false;
            }
            else
            {
                this.AddPostVote(PostID, GolferID, VoteType.Like);
                return true;
            }
        }


        public bool LikeComment(Guid PostID, Guid CommentID, Guid GolferID)
        {
            PostVote postVote = _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.PostID == PostID && pv.CommentID == CommentID && pv.Type == VoteType.Like).FirstOrDefault();
            if (postVote != null)
            {
                _postVoteRepository.Remove(postVote);
                return false;
            }
            else
            {
                this.AddCommentVote(PostID, CommentID, GolferID, VoteType.Like);
                return true;
            }
        }

        public PostVote GetPostVote(Guid PostID, Guid GolferID, VoteType type)
        {
            return _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.PostID == PostID && pv.CommentID == null && pv.Type == type).FirstOrDefault();
        }

        public PostVote GetCommentVote(Guid PostID, Guid CommentID, Guid GolferID, VoteType type)
        {
            return _postVoteRepository.Find(pv => pv.CreatedBy == GolferID && pv.CommentID == CommentID && pv.PostID == PostID && pv.Type == type).FirstOrDefault();
        }



        public bool GetCurrentGolferLikeStatusOfPost(Guid PostID, Guid GolferID)
        {
            int count = _postVoteRepository.CountAll(pv => pv.CreatedBy == GolferID && pv.PostID == PostID && pv.CommentID == null && pv.Type == VoteType.Like);
            return count != 0 ? true : false;
        }

        public bool GetCurrentGolferLikeStatusOfComment(Guid CommentID, Guid GolferID)
        {
            int count = _postVoteRepository.CountAll(pv => pv.CreatedBy == GolferID && pv.CommentID == CommentID && pv.Type == VoteType.Like);
            return count != 0 ? true : false;
        }

        public int CountCommentVoteDetail(Guid CommentID, VoteType type)
        {
            return _postVoteRepository.CountAll(pv => pv.CommentID == CommentID && pv.Type == type);
        }
    }
}
