using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Domain.GolferData;
using Golf.EntityFrameworkCore;
using Golf.Core.Common.Golfer;
using AutoMapper;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Golfer;
using Golf.Core.Dtos.Controllers.GolfersController.Responses;
using Golf.Domain.Shared.Relationship;
using Golf.Domain.Shared.Golfer.UserSetting;
using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.GolfersController.Requests.Setting;

namespace Golf.Services
{
    public class GolferService
    {
        private readonly GolferRepository _golferRepository;
        private readonly CourseRepository _courseRepository;
        private readonly GroupRepository _groupRepository;
        private readonly LocationRepository _locationRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly RelationshipRepository _relationshipRepository;
        private readonly StatisticService _statisticService;
        private readonly RelationshipService _relationshipService;
        private readonly IMapper _mapper;

        public GolferService(GroupRepository groupRepository, RelationshipService relationshipService, StatisticService statisticService, RelationshipRepository relationshipRepository, ScorecardRepository scorecardRepository, IMapper mapper, DatabaseTransaction databaseTransaction, LocationRepository locationRepository, CourseRepository courseRepository, GolferRepository golferRepository)
        {
            _groupRepository = groupRepository;
            _statisticService = statisticService;
            _golferRepository = golferRepository;
            _courseRepository = courseRepository;
            _databaseTransaction = databaseTransaction;
            _locationRepository = locationRepository;
            _mapper = mapper;
            _relationshipService = relationshipService;
            _scorecardRepository = scorecardRepository;
            _relationshipRepository = relationshipRepository;
        }

        /// <summary>
        /// Lấy dữ liệu người dùng
        /// </summary>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <returns></returns>
        public Golfer GetGolfer(Guid GolferID)
        {
            var golfer = _golferRepository.Get(GolferID);
            return golfer;
        }

        /// <summary>
        /// Lấy dữ liệu người dùng thu gọn
        /// </summary>
        /// <param name="GolferID">Định danh người dùng</param>
        /// <returns></returns>
        public MinimizedGolfer GetMinimizedGolfer(Guid GolferID)
        {
            var golfer = _golferRepository.GetMinimizedGolfer(GolferID);
            return golfer;
        }

        /// <summary>
        /// Tìm kiếm người dùng theo tên
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="searchKey">Từ khóa tìm kiếm</param>
        /// <param name="startIndex"> Số thứ tự vị trí lấy đầu tiên</param>
        /// <returns></returns>
        public async Task<List<MinimizedGolferResponse>> SearchGolferByName(Guid currentID, string searchKey, int startIndex)
        {
            var result = new List<MinimizedGolferResponse>();
            var golfers = _golferRepository.Find(gf => gf.FirstName.ToLower().Contains(searchKey) || gf.LastName.ToLower().Contains(searchKey) || gf.PhoneNumber.ToLower().Contains(searchKey)|| gf.Email.ToLower().Contains(searchKey)).Skip(startIndex).Take(Const.PageSize).ToList();
            if (golfers.Count() == 0)
                return new List<MinimizedGolferResponse>();
            foreach (var i in golfers)
            {
                var minimizedGolferResponse = new MinimizedGolferResponse();
                var relation = _relationshipRepository.Find(rl => (rl.FriendID == i.Id && rl.GolferID == currentID) || (rl.GolferID == i.Id && rl.FriendID == currentID));
                minimizedGolferResponse.Golfer = _mapper.Map<MinimizedGolfer>(i);
                minimizedGolferResponse.Relationship = _relationshipService.GetRelationship(currentID, i.Id);
                minimizedGolferResponse.NumberOfMutualFriends =await this.GetNumberOfMutualFriends(currentID, i.Id);
                result.Add(minimizedGolferResponse);
            }
            return result;
        }
        public async Task<int> GetNumberOfMutualFriends(Guid currentID,Guid golferID)
        {
            var golferFriends1 = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && (golferID==r.GolferID&& r.FriendID!=currentID&& r.FriendID!=golferID)).GroupBy(r=>r.FriendID).Select(r=>new {Key=r.Key,value=r.First().FriendID});
            var golferFriends2  = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && (golferID == r.FriendID&& r.GolferID!= currentID&& r.GolferID!= golferID)).GroupBy(r => r.GolferID).Select(r => new { Key = r.Key, value = r.First().GolferID });
            var currentFriends1 = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && (currentID == r.GolferID&& r.FriendID!= golferID&& r.FriendID!= currentID)).GroupBy(r => r.FriendID).Select(r => new { Key = r.Key, value = r.First().FriendID });
            var currentFriends2 = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && (currentID == r.FriendID&& r.GolferID!= golferID&&r.GolferID!= currentID)).GroupBy(r => r.GolferID).Select(r => new { Key = r.Key, value = r.First().GolferID });
            golferFriends1 = golferFriends1.Concat(golferFriends2).ToList();
            currentFriends1 = currentFriends1.Concat(currentFriends2).ToList();
            var tmp = golferFriends1.Concat(currentFriends1).GroupBy(t=>t.value).Select(t=>new { Key=t.Key,Count=t.Count()}).ToList();
            return tmp.Count(t => t.Count == 2);
        }
        /// <summary>
        /// Lấy danh sách gợi ý kết bạn
        /// </summary>
        /// <param name="currentID">Định danh người dùng</param>
        /// <param name="phoneNumber">Số điện thoại</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        //public List<MinimizedGolferResponse> SuggestAddFriend(Guid currentID, List<string> phoneNumber, int startIndex)
        //{
        //    var result = new List<MinimizedGolferResponse>();
        //    var golfers = _golferRepository.Find(gf => phoneNumber.Contains(gf.PhoneNumber)).Skip(startIndex).Take(Const.PageAmount).ToList();
        //    if (golfers.Count() == 0)
        //        return new List<MinimizedGolferResponse>();
        //    foreach (var i in golfers)
        //    {
        //        var minimizedGolferResponse = new MinimizedGolferResponse();
        //        var relation = _relationshipRepository.Find(rl => (rl.FriendID == i.Id && rl.GolferID == currentID) || (rl.GolferID == i.Id && rl.FriendID == currentID));
        //        if (relation.Count() > 0 && ((int)relation.First().Status == (int)RelationshipStatus.IsFriend || (int)relation.First().Status == (int)RelationshipStatus.Block))
        //            break;

        //        minimizedGolferResponse.Golfer = _mapper.Map<MinimizedGolfer>(i);
        //        minimizedGolferResponse.Relationship = _relationshipService.GetRelationship(currentID, i.Id);

        //        result.Add(minimizedGolferResponse);
        //    }
        //    return result;
        //} 
        public List<MinimizedGolferResponse> SuggestAddFriend(Guid currentID, int startIndex)
        {
            var result = new List<MinimizedGolferResponse>();
            var friends = _relationshipRepository.FindWithGolfer(r => r.FriendID == currentID || r.GolferID == currentID).ToList();
            List<Guid> friendIDs = new List<Guid>();
            foreach (var i in friends)
            {
                if (i.FriendID == currentID)
                {
                    friendIDs.Add(i.GolferID);
                }
                else
                {
                    friendIDs.Add(i.FriendID);
                }
            }
            var golfers = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && (friendIDs.Contains(r.GolferID)&&friendIDs.Contains(r.FriendID)==false&&r.FriendID!=currentID)).GroupBy(r => r.FriendID).Select(g => new {Key=g.Key ,Count= g.Count() });
            var tmp = _relationshipRepository.FindWithGolfer(r => r.Status == RelationshipStatus.IsFriend && ( friendIDs.Contains(r.FriendID) &&friendIDs.Contains(r.GolferID) && r.GolferID != currentID)).GroupBy(r=>r.GolferID).Select(g => new { Key = g.Key, Count = g.Count() });
            golfers= golfers.Concat(tmp).OrderBy(g=>g.Count).ToList();
            if (golfers.Count() == 0)
                return new List<MinimizedGolferResponse>();
            foreach (var i in golfers)
            {
                var minimizedGolferResponse = new MinimizedGolferResponse();
                minimizedGolferResponse.Golfer = _mapper.Map<MinimizedGolfer>(_golferRepository.Get(i.Key));
                minimizedGolferResponse.NumberOfMutualFriends = i.Count;
                minimizedGolferResponse.Relationship = "IsNotFriend";

                result.Add(minimizedGolferResponse);
            }
            return result;
        }

        /// <summary>
        /// Xếp hạng người dùng theo Net, Gross, Handicap
        /// </summary>
        /// <param name="rankFilter">Bộ lọc</param>
        /// <param name="startIndex"></param>
        /// <param name="curentID">Định danh người dùng</param>
        /// <returns></returns>
        public RankingResponse Ranking(RankFilter rankFilter, int startIndex, Guid curentID)
        {
            var response = new RankingResponse();
            bool checkRankingCurrentGolfer = _statisticService.GetGolferTotalMatches(curentID) > 10;
            switch (rankFilter)
            {
                case RankFilter.BestGross://lọc theo điểm tổng
                    {
                        var tmp = _scorecardRepository.Find(sc => sc.CreatedDate.Date == DateTime.Now.AddDays(-1).Date).GroupBy(s => s.OwnerID).Select(s => new { GolferID = s.Key, MinGross = s.Min(l => l.Grosses) }).OrderBy(s => s.MinGross);
                        if (tmp.Count() < 0)
                        {
                            return new RankingResponse();
                        }
                        var golferRanking = tmp.ToList();
                        var result = golferRanking.FindAll(gf => _statisticService.GetGolferTotalMatches(gf.GolferID) > 10).OrderBy(gf => gf.MinGross).Skip(startIndex).Take(Const.PageSize).ToList();
                        foreach (var i in result)
                        {
                            var scocard = _scorecardRepository.Find(s => s.Grosses == i.MinGross).First();
                            var courseName = _courseRepository.Get(scocard.CourseID).Name;
                            response.RankingGolfers.Add(new RankingGolfer() { Golfer = _mapper.Map<MinimizedGolfer>(_golferRepository.Get(i.GolferID)), CourseName = courseName, BestGross = i.MinGross });
                            if (checkRankingCurrentGolfer == false)
                            {
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.NotEnough;
                            }
                            else if (_scorecardRepository.Find(sc => sc.CreatedDate.Date == DateTime.Now.AddDays(-1).Date && sc.OwnerID == curentID).Count() > 0)
                            {
                                var t = golferRanking.FindIndex(sc => sc.GolferID == curentID) + 1;
                                response.RankingCurrentGolfer.Rank = t;
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.HasRank;
                                response.RankingCurrentGolfer.BestGross = golferRanking.Find(golferRanking => golferRanking.GolferID == curentID).MinGross;
                            }
                            else
                            {
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.NoPlay;
                            }
                        }
                        return response;
                    }
                case RankFilter.BestNet://lọc theo điểm thực
                    {
                        var tmp = _scorecardRepository.Find(sc => sc.CreatedDate.Date == DateTime.Now.AddDays(-1).Date).GroupBy(s => s.OwnerID).Select(s => new { GolferID = s.Key, MinNet = s.Min(l => l.RealGrosses) }).OrderBy(s => s.MinNet);
                        if (tmp.Count() < 0)
                        {
                            return new RankingResponse();
                        }
                        var golferRanking = tmp.ToList();
                        var resulưt = golferRanking.FindAll(gf => _statisticService.GetGolferTotalMatches(gf.GolferID) > 10);
                        var result = golferRanking.FindAll(gf => _statisticService.GetGolferTotalMatches(gf.GolferID) > 10).OrderBy(gf => gf.MinNet).Skip(startIndex).Take(Const.PageSize).ToList();
                        foreach (var i in result)
                        {
                            var scocard = _scorecardRepository.Find(s => s.RealGrosses == i.MinNet).First();
                            var courseName = _courseRepository.Get(scocard.CourseID).Name;
                            response.RankingGolfers.Add(new RankingGolfer() { Golfer = _mapper.Map<MinimizedGolfer>(_golferRepository.Get(i.GolferID)), CourseName = courseName, BestNet = i.MinNet });
                            if (checkRankingCurrentGolfer == false)
                            {
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.NotEnough;
                            }
                            else if (_scorecardRepository.Find(sc => sc.CreatedDate.Date == DateTime.Now.AddDays(-1).Date && sc.OwnerID == curentID).Count() > 0)
                            {
                                var t = golferRanking.FindIndex(sc => sc.GolferID == curentID) + 1;
                                response.RankingCurrentGolfer.Rank = t;
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.HasRank;
                                response.RankingCurrentGolfer.BestNet = golferRanking.Find(golferRanking => golferRanking.GolferID == curentID).MinNet;
                            }
                            else
                            {
                                response.RankingCurrentGolfer.Type = RankingCurrentGolferType.NoPlay;
                            }
                        }
                        return response;
                    }
                case RankFilter.ByHandicap://lọc theo điểm chấp
                    {
                        var tmp = _golferRepository.GetAll().OrderBy(g => g.Handicap);
                        if (tmp.Count() <= 0)
                        {
                            return new RankingResponse();
                        }
                        var count = 0;
                        var hasGolfer = false;
                        foreach (var i in tmp)
                        {
                            if (_statisticService.GetGolferTotalMatches(i.Id) > 10)
                            {
                                response.RankingGolfers.Add(new RankingGolfer { Golfer = _mapper.Map<MinimizedGolfer>(i) });
                                count = count + 1;
                            }
                            if (count > 9) break;
                        }
                        if (checkRankingCurrentGolfer)
                        {
                            response.RankingCurrentGolfer.Rank = tmp.ToList().FindAll(g => _statisticService.GetGolferTotalMatches(g.Id) > 10).OrderBy(g => g.Handicap).ToList().FindIndex(g => g.Id == curentID) + 1;
                            response.RankingCurrentGolfer.Type = RankingCurrentGolferType.HasRank;
                        }
                        else
                        {
                            response.RankingCurrentGolfer.Type = RankingCurrentGolferType.NotEnough;
                        }
                        return response;
                    }
                default:
                    return new RankingResponse();
            }
        }

        /// <summary>
        /// Theeo sân hoặc bỏ sân vào danh sách yêu thích
        /// </summary>
        /// <param name="golferID"></param>
        /// <param name="courseOrLocationID"></param>
        /// <returns></returns>
        public async Task<bool> FavoriteCourse(Guid golferID, Guid courseOrLocationID)
        {
            var location = _locationRepository.Get(courseOrLocationID);
            _databaseTransaction.BeginTransaction();
            if (location != null)
            {
                var courses = _courseRepository.Find(c => c.Location.ID == location.ID);
                if (courses.Count() == 0)
                    return false;
                var golfer = _golferRepository.Get(golferID);
                if (golfer == null)
                    return false;
                var tmp = courses.ToList();
                foreach (var i in tmp)
                {
                    if (golfer.CourseFavorites.Find(c => c == i.ID) == Guid.Empty)
                        golfer.CourseFavorites.Add(i.ID);
                    else
                    {
                        golfer.CourseFavorites.RemoveAll(cid => tmp.Select(c => c.ID).Contains(cid));
                        break;
                    }
                }
                _golferRepository.SafeUpdate(golfer);
                await _databaseTransaction.Commit();
                return true;
            }
            else
            {
                var course = _courseRepository.Get(courseOrLocationID);
                var golfer = _golferRepository.Get(golferID);
                if (golfer == null || course == null)
                    return false;
                if (golfer.CourseFavorites.Find(c => c == courseOrLocationID) == Guid.Empty)
                    golfer.CourseFavorites.Add(courseOrLocationID);
                else
                    golfer.CourseFavorites.Remove(courseOrLocationID);
                _golferRepository.SafeUpdate(golfer);
                await _databaseTransaction.Commit();

                return true;
            }

        }

        /// <summary>
        /// Lấy danh sách người dùng với danh sách định danh đầu vào
        /// </summary>
        /// <param name="golferIDs">Danh sách định danh</param>
        /// <returns></returns>
        public List<Golfer> GetGolfers(List<Guid> golferIDs)
        {
            if (golferIDs.Count() < 0)
                return new List<Golfer>();
            var golfers = _golferRepository.Find(golfer => golferIDs.Contains(golfer.Id)).ToList();
            return golfers;
        }

        /// <summary>
        /// Lấy danh sách người dùng bằng danh sách định danh 
        /// </summary>
        /// <param name="golferIDs"></param>
        /// <returns></returns>
        public List<MinimizedGolfer> GetMinimizedGolfers(List<Guid> golferIDs)
        {
            if (golferIDs.Count() < 0)
                return new List<MinimizedGolfer>();
            var golfers = _golferRepository.Find(golfer => golferIDs.Contains(golfer.Id)).ToList();
            var result = new List<MinimizedGolfer>();
            foreach (var i in golfers)
            {
                result.Add(_mapper.Map<MinimizedGolfer>(i));
            }
            return result;
        }
        public List<MiniGolfer> GetMiniGolfers(List<Guid> golferIDs)
        {
            if (golferIDs.Count() < 0)
                return new List<MiniGolfer>();
            var golfers = _golferRepository.Find(golfer => golferIDs.Contains(golfer.Id)).ToList();
            var result = new List<MiniGolfer>();
            foreach (var i in golfers)
            {
                result.Add(new MiniGolfer() { Golfer = _mapper.Map<MinimizedGolfer>(i) });
            }
            return result;
        }

        //golfer setting
        public UserSettingRequest MySetting(Guid curentID)
        {
            var golfer = _golferRepository.Get(curentID);
            if (golfer == null)
                throw new NotFoundException("Not found user!");
            return new UserSettingRequest() { LanguageSetting = golfer.Setting.LanguageSetting, NotificationSetting = golfer.Setting.NotificationSetting, VideoSetting = golfer.Setting.VideoSetting };
        }

        public GroupSetting MyGroupSetting(Guid curentID, Guid groupID)
        {
            var golfer = _golferRepository.Get(curentID);
            if (golfer == null)
                throw new NotFoundException("Not found user!");
            var group = _groupRepository.Get(groupID);
            if (group == null)
                throw new NotFoundException("Not found group!");
            if (golfer.Setting.GroupSettings.Find(s => s.GroupID == groupID) == null)
            {
                golfer.Setting.GroupSettings.Add(new GroupSetting() { GroupID = groupID });
                _golferRepository.UpdateEntity(golfer);
            }
            return golfer.Setting.GroupSettings.Find(s => s.GroupID == groupID);
        }
        public GroupSetting SafeMyGroupSetting(Golfer golfer, Guid groupID)
        {
            if (golfer.Setting.GroupSettings.Find(s => s.GroupID == groupID) == null)
            {
                golfer.Setting.GroupSettings.Add(new GroupSetting() { GroupID = groupID });
                _golferRepository.UpdateEntity(golfer);
            }
            return golfer.Setting.GroupSettings.Find(s => s.GroupID == groupID);
        }

        public UserSettingRequest UpdateMySetting(Guid curentID, UserSettingRequest userSettingRequest)
        {
            var golfer = _golferRepository.Get(curentID);
            if (golfer == null)
                throw new NotFoundException("Not found user!");
            golfer.Setting = new UserSetting() { LanguageSetting = userSettingRequest.LanguageSetting, NotificationSetting = userSettingRequest.NotificationSetting, VideoSetting = userSettingRequest.VideoSetting };
            _golferRepository.UpdateEntity(golfer);
            return userSettingRequest;
        }

        public GroupSetting UpdateMyGroupSetting(Guid curentID, GroupSetting groupSetting)
        {
            var golfer = _golferRepository.Get(curentID);
            if (golfer == null)
                throw new NotFoundException("Not found user!");
            var group = _groupRepository.Get(groupSetting.GroupID);
            if (group == null)
                throw new NotFoundException("Not found group!");
            var gSetting = golfer.Setting.GroupSettings.Find(s => s.GroupID == groupSetting.GroupID);
            if (gSetting == null)
            {
                golfer.Setting.GroupSettings.Add(new GroupSetting() { GroupID = groupSetting.GroupID, NotificationGroupSetting = groupSetting.NotificationGroupSetting });
            }
            else
            {
                golfer.Setting.GroupSettings.Remove(gSetting);
                golfer.Setting.GroupSettings.Add(groupSetting);
            }
            _golferRepository.UpdateEntity(golfer);
            return golfer.Setting.GroupSettings.Find(s => s.GroupID == groupSetting.GroupID);
        }
    }
}