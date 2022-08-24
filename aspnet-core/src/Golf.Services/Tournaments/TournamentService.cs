using AutoMapper;
using Golf.Core.Common.Golfer;
using Golf.Core.Common.Scorecard;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Dtos.Controllers.TournamentController.Request;
using Golf.Core.Dtos.Controllers.TournamentController.Response;
using Golf.Core.Dtos.Groups;
using Golf.Core.Exceptions;
using Golf.Domain.Post;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Post;
using Golf.Domain.Shared.Tuanament;
using Golf.Domain.SocialNetwork;
using Golf.Domain.Tournaments;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Tournaments
{
    public class TournamentService
    {
        private readonly TournamentRepository _tournamentRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly GroupMemberService _groupMemberService;
        private readonly GroupRepository _groupRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly TournamentMemberRepository _tournamentMemberRepository;
        private readonly GolferRepository _golferRepository;
        private readonly GolferService _golferService;
        private readonly ProfileRepository _profileRepository;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly ScorecardService _scorecardService;
        private readonly NotificationService _notificationService;
        private readonly PostRepository _postRepository;
        private readonly IMapper _mapper;
        public TournamentService(PostRepository postRepository, GroupMemberService groupMemberService, NotificationService notificationService, ScorecardService scorecardService, GolferRepository golferRepository, GolferService golferService, ProfileRepository profileRepository, ScorecardRepository scorecardRepository, GroupMemberRepository groupMemberRepository, GroupRepository groupRepository, DatabaseTransaction databaseTransaction, TournamentRepository tournamentRepository, TournamentMemberRepository tournamentMemberRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _groupMemberService = groupMemberService;
            _notificationService = notificationService;
            _golferRepository = golferRepository;
            _golferService = golferService;
            _profileRepository = profileRepository;
            _scorecardRepository = scorecardRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;
            _scorecardService = scorecardService;
            _databaseTransaction = databaseTransaction;
            // _groupRepository = groupRepository;
            _mapper = mapper;
            _tournamentMemberRepository = tournamentMemberRepository;
            _tournamentRepository = tournamentRepository;
        }

        /// <summary>
        /// Lọc danh sách giả đấu theo tiêu chí(All,Request,Joined,Owner,Registered)
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiệ thời</param>
        /// <param name="filter">Bộ lọc</param>
        /// <param name="date">bộ lọc thời gian(có thể có hoặc không)</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>Danh sách kết quả lọc</returns>
        public List<TournamentResponse> FilterTournamment(Guid currentID, TournamentFilter filter, DateTime? date,string? searchKey, int startIndex)
        {
            switch (filter)
            {
                case TournamentFilter.All:
                    {
                        var tournaments = _tournamentRepository.Find(t => ((t.TournamentName.ToLower().Trim().Contains(searchKey.Trim().ToLower()) || t.OrganizationalUnitName.Trim().ToLower().Contains(searchKey.Trim().ToLower()))) &&t.Status != TournamentStatus.Requested && t.Status != TournamentStatus.Rejected && (date == null ? true : DateTime.Compare((DateTime)date, t.DateTime.Date) == 0) && DateTime.Compare(DateTime.Now, t.DateTime) < 0 && t.OwnerID != currentID).OrderByDescending(t => t.DateTime).Skip(startIndex).Take(Const.PageSize);
                        if (tournaments.Count() == 0)
                            return new List<TournamentResponse>();
                        return this.GetTournammentRespones(currentID, tournaments.ToList());
                    }
                case TournamentFilter.Owner:
                    {
                        var tournaments = _tournamentRepository.Find(t =>((t.TournamentName.ToLower().Trim().Contains(searchKey.Trim().ToLower()) || t.OrganizationalUnitName.Trim().ToLower().Contains(searchKey.Trim().ToLower())))&& t.OwnerID == currentID).OrderByDescending(t => t.DateTime).Skip(startIndex).Take(Const.PageSize);
                        if (tournaments.Count() == 0)
                            return new List<TournamentResponse>();
                        return this.GetTournammentRespones(currentID, tournaments.ToList());
                    }
                case TournamentFilter.Registered:
                    {
                        var tournaments = _tournamentMemberRepository.FindIncludeTournament(t =>((t.Tuanament.TournamentName.ToLower().Trim().Contains(searchKey.Trim().ToLower()) || t.Tuanament.OrganizationalUnitName.Trim().ToLower().Contains(searchKey.Trim().ToLower())))&& t.GolferID == currentID && t.MemberStatus == TournamentMemberStatus.Joined && (date == null ? true : DateTime.Compare((DateTime)date, t.Tuanament.DateTime.Date) == 0)).OrderByDescending(tm => tm.Tuanament.DateTime).Skip(startIndex).Take(Const.PageSize);
                        if (tournaments.Count() == 0)
                            return new List<TournamentResponse>();
                        return this.GetTournammentRespones(currentID, tournaments.Select(tm => tm.Tuanament).ToList());
                    }
                case TournamentFilter.Request:
                    {
                        var tournaments = _tournamentMemberRepository.FindIncludeTournament(t => (t.Tuanament.TournamentName.ToLower().Trim().Contains(searchKey.Trim().ToLower()) || t.Tuanament.OrganizationalUnitName.Trim().ToLower().Contains(searchKey.Trim().ToLower())) && t.GolferID == currentID && t.MemberStatus == TournamentMemberStatus.Requested && (date == null ? true : DateTime.Compare((DateTime)date, t.Tuanament.DateTime.Date) == 0)).OrderBy(tm => tm.Tuanament.DateTime).Skip(startIndex).Take(Const.PageSize);
                        if (tournaments.Count() == 0)
                            return new List<TournamentResponse>();
                        return this.GetTournammentRespones(currentID, tournaments.Select(tm => tm.Tuanament).ToList());
                    }
                default:
                    {
                        throw new BadRequestException("Invalid filter value");
                    }
            }

        }

        /// <summary>
        /// Tìm kiếm giải đấu theo tên
        /// </summary>
        /// <param name="currentID">định danh người dùng</param>
        /// <param name="searchKey">Từ khóa tìm kiếm</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>Danh sách kết quả tìm kiếm</returns>
        public List<TournamentResponse> Search(Guid currentID, string searchKey, int startIndex)
        {
            var tournaments = _tournamentRepository.Find(t => t.Status != TournamentStatus.Requested && t.Status != TournamentStatus.Rejected && DateTime.Compare(DateTime.Now, t.DateTime) < 0 && (t.TournamentName.ToLower().Trim().Contains(searchKey.Trim().ToLower()) || t.OrganizationalUnitName.Trim().ToLower().Contains(searchKey.Trim().ToLower()))).OrderBy(t => t.DateTime).Skip(startIndex).Take(Const.PageSize);
            if (tournaments.Count() == 0)
                return new List<TournamentResponse>();
            return this.GetTournammentRespones(currentID, tournaments.ToList());
        }

        /// <summary>
        /// Gửi yêu cầu tạo giải đáu
        /// </summary>
        /// <param name="currentID">đạnh danh người gửi</param>
        /// <param name="tournamentRequest">Dữ liệu tạo giải đấu</param>
        /// <returns>Kết quả tạo</returns>
        public TournamentResponse Add(Guid currentID, TournamentRequest tournamentRequest)
        {
            Tournament tournament = new Tournament();
            _mapper.Map(tournamentRequest, tournament);
            tournament.DateTime = tournamentRequest.Date.Date + tournamentRequest.Time.TimeOfDay;
            tournament.OwnerID = currentID;
            tournament.Status = TournamentStatus.Requested;
            _tournamentRepository.Add(tournament);
            _notificationService.NotificationSendTournamentRequest(currentID, tournament.ID).Wait();
            return this.GetTournammentRespone(currentID, tournament); 
        }

        /// <summary>
        /// sửa thông tin giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng</param>
        /// <param name="tournamentID">ĐỊnh danh giải đấu</param>
        /// <param name="tournamentRequest">Dữ liệu cập nhật</param>
        /// <returns></returns>
        public bool Edit(Guid currentID, Guid tournamentID, TournamentRequest tournamentRequest)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID || tournament.Status != TournamentStatus.Requested || tournament.Status != TournamentStatus.Rejected)
            {
                throw new BadRequestException("Not allow!");
            }
            _mapper.Map(tournamentRequest, tournament);
            tournament.DateTime = tournamentRequest.Date.Date + tournamentRequest.Time.TimeOfDay;
            _tournamentRepository.UpdateEntity(tournament);
            return true;
        }
        /// <summary>
        /// Hủy yêu cầu tạo giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns>Kết quả hủy</returns>
        public bool Cancel(Guid currentID, Guid tournamentID)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID)
            {
                throw new BadRequestException("Not allow!");
            }
            if (tournament.Status == TournamentStatus.Requested)
            {
                _tournamentRepository.RemoveEntity(tournament);
                return true;

            }
            else throw new BadRequestException("Can not Cancel!");

        }

        /// <summary>
        /// Xóa giải đấu 
        /// </summary>
        /// <param name="currentID">định danh người dùng hiện thời</param>
        /// <param name="tournamentID">ĐỊnh danh giải đáu</param>
        /// <returns></returns>
        public async Task Delete(Guid currentID, Guid tournamentID)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var tournament = _tournamentRepository.Get(tournamentID);
                List<GroupMember> members = new List<GroupMember>();
                List<TournamentMember> tounamentMembers = new List<TournamentMember>();
                if (tournament == null)
                {
                    throw new NotFoundException("Tournament not found!");
                }
                if (tournament.OwnerID != currentID)
                {
                    throw new ForbiddenException("Not allow!");
                }
                tounamentMembers = _tournamentMemberRepository.Find(tm => tm.TuornamentID == tournamentID).ToList();
                if(tournament.GroupID!=null&&tournament.GroupID!=Guid.Empty)
                {
                    var group = _groupRepository.Get((Guid)tournament.GroupID);
                    _groupRepository.SafeRemove(group);
                    members = _groupMemberRepository.Find(gm => gm.GroupID == tournament.GroupID).ToList();
                    _groupMemberRepository.SafeRemoveRange(members);
                    _postRepository.SafeRemoveRange(_postRepository.Find(p => p.GroupID == group.ID).ToList());
                }    
                _tournamentRepository.SafeRemove(tournament);
                await _databaseTransaction.Commit();
                //noitification
                if (tournament.GroupID != null && tournament.GroupID != Guid.Empty)
                {
                    foreach (var i in members)
                    {
                        if (i.Status != MemberStatus.Request)
                        {
                            _notificationService.NotificationRemoveGroup(i.GolferID, (Guid)tournament.GroupID);
                        }
                    }
                }    
                foreach(var i in tounamentMembers)
                {
                    _notificationService.NotificationDeleteTournament(i.GolferID, tournamentID);
                }    
                //notification
            }
            catch(Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Gửi yêu cầu tham gia giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns></returns>
        public bool AddJoindRequest(Guid currentID, Guid tournamentID)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID && (tournament.Status == TournamentStatus.Requested || tournament.Status == TournamentStatus.Rejected))
            {
                throw new ForbiddenException("Not allow!");
            }
            var tm = _tournamentMemberRepository.Find(tm => tm.GolferID == currentID && tm.TuornamentID == tournamentID);
            if (tm.Count() > 0)
            {
                throw new BadRequestException("User send request or joined");
            }
            TournamentMember tournamentMember = new TournamentMember();
            tournamentMember.GolferID = currentID;
            tournamentMember.TuornamentID = tournamentID;
            tournamentMember.MemberStatus = TournamentMemberStatus.Requested;
            _tournamentMemberRepository.Add(tournamentMember);
            _notificationService.NotificationSendTournamentMemberRequest(currentID, tournamentID);
            return true;
        }
        /// <summary>
        /// Xác nhận các yêu cầu tham gia
        /// </summary>
        /// <param name="currentID">Định danh người dùng</param>
        /// <param name="memberID">Định danh yêu cầu tham gia</param>
        /// <returns>Kết quả</returns>
        public async Task<bool> ConfirmJoindRequest(Guid currentID, Guid memberID)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                var tournamentMember = _tournamentMemberRepository.Get(memberID);
                if (tournamentMember == null)
                {
                    throw new NotFoundException("Not found request");
                }
                var tournament = _tournamentRepository.Get(tournamentMember.TuornamentID);
                if (tournament.OwnerID != currentID)
                {
                    throw new ForbiddenException("Not allow!");
                }
                if (tournamentMember.MemberStatus == TournamentMemberStatus.Requested)
                {
                    tournamentMember.MemberStatus = TournamentMemberStatus.Joined;
                    _tournamentMemberRepository.SafeUpdate(tournamentMember);
                    _notificationService.NotificationConfirmTournamentMember(currentID, tournamentMember.GolferID, tournamentMember.TuornamentID);
                    if (tournament.GroupID != null)
                    {
                        var group = _groupRepository.Get((Guid)tournament.GroupID);
                        if (group != null)
                        {
                            await _groupMemberService.SafeAddGroupMember(currentID,group,tournamentMember.GolferID, false);
                        }
                    }
                    await _databaseTransaction.Commit();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// từ chối yêu cầu tham gia giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="memberID">Định danh thành viên</param>
        /// <returns></returns>
        public async Task<bool> RejectJoindRequest(Guid currentID, Guid memberID)
        {
            _databaseTransaction.BeginTransaction();
            try
            {
                var tournamentMember = _tournamentMemberRepository.Get(memberID);
                if (tournamentMember == null)
                {
                    throw new NotFoundException("Not found request");
                }
                var tournament = _tournamentRepository.Get(tournamentMember.TuornamentID);
                if (tournament.OwnerID != currentID)
                {
                    throw new ForbiddenException("Not allow!");
                }
                if (tournamentMember.MemberStatus == TournamentMemberStatus.Requested)
                {
                    tournamentMember.MemberStatus = TournamentMemberStatus.Rejected;
                    _tournamentMemberRepository.SafeUpdate(tournamentMember);
                    if (tournament.GroupID != null)
                    {
                        var group = _groupRepository.Get((Guid)tournament.GroupID);
                        if (group != null)
                        {
                            await _groupMemberService.SafeAddGroupMember(currentID,group,tournamentMember.GolferID, false);
                        }
                    }
                    await _databaseTransaction.Commit();
                    _notificationService.NotificationRejectTournamentMember(currentID, tournamentMember.GolferID, tournamentMember.TuornamentID);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Hủy yêu cầu tham gia giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <returns>Kết quả</returns>
        public bool CancelJoindRequest(Guid currentID, Guid tournamentID)
        {
            var tournamentMembers = _tournamentMemberRepository.Find(tm => tm.GolferID == currentID && tm.TuornamentID == tournamentID && tm.MemberStatus == TournamentMemberStatus.Requested);
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournamentMembers.Count() <= 0)
            {
                throw new NotFoundException("Not found request");
            }
            //tournamentMembers.First().MemberStatus = TournamentMemberStatus.Rejected;
            _tournamentMemberRepository.RemoveEntity(tournamentMembers.First());
            return true;
        }
    
        /// <summary>
        /// Lấy các yêu cầu tham gia giải đấu
        /// </summary>
        /// <param name="currentID">định danh người dùng hiện thời</param>
        /// <param name="tournamentID">định danh giải đấu</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>Danh sách yêu cầu tham gia giải đấu</returns>
        public List<TournamentMemberResponse> GetJoinRequest(Guid currentID, Guid tournamentID, int startIndex)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID)
            {
                throw new ForbiddenException("Not allow!");
            }
            var tournamentMembers = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.TuornamentID == tournamentID && tm.MemberStatus == TournamentMemberStatus.Requested).Skip(startIndex).Take(Const.PageSize);
            List<TournamentMemberResponse> tournamentMemberResponses = new List<TournamentMemberResponse>();
            foreach (var i in tournamentMembers)
            {
                tournamentMemberResponses.Add(GetTournamentMemberResponse(i));
            }
            return tournamentMemberResponses;
        }
        /// <summary>
        /// lấy danh sách thành viên đã tham gia giải đấu
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="tournamentID">Định danh giải đấu</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>danh sách thành viên</returns>
        public List<TournamentMemberResponse> GetMemberTournament(Guid currentID, Guid tournamentID, int startIndex)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID)
            {
                throw new ForbiddenException("Not allow!");
            }
            var tournamentMembers = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.TuornamentID == tournamentID && tm.MemberStatus == TournamentMemberStatus.Joined).Skip(startIndex).Take(Const.PageSize).ToList();
            List<TournamentMemberResponse> tournamentMemberResponses = new List<TournamentMemberResponse>();
            foreach (var i in tournamentMembers)
            {
                tournamentMemberResponses.Add(GetTournamentMemberResponse(i));
            }
            return tournamentMemberResponses;
        }
        public List<TournamentMemberResponse> SearchMember(Guid currentID, Guid tournamentID, string searchKey, TournamentMemberStatus tournamentMemberStatus, int startIndex)
        {
            if (searchKey == null) searchKey = "";
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            var currentMember = _tournamentMemberRepository.Find(tm => tm.MemberStatus == TournamentMemberStatus.Joined && tm.TuornamentID == tournamentID).FirstOrDefault();
      
            switch (tournamentMemberStatus)
            {
                case TournamentMemberStatus.Joined:
                    {
                        if(currentID==tournament.OwnerID||currentMember!=null)
                        {
                            break;
                        }
                        else
                        {
                            throw new ForbiddenException("Not allow");
                        }
                    }
                case TournamentMemberStatus.Rejected:
                    {
                        if (currentID == tournament.OwnerID )
                        {
                            break;
                        }
                        else
                        {
                            throw new ForbiddenException("Not allow");
                        }
                    }
                case TournamentMemberStatus.Requested:
                    {
                        if (currentID == tournament.OwnerID || currentMember != null)
                        {
                            break;
                        }
                        else
                        {
                            throw new ForbiddenException("Not allow");
                        }
                    }
                default:
                    return new List<TournamentMemberResponse>();
            }    

            if (tournament.OwnerID != currentID)
            {
                throw new ForbiddenException("Not allow!");
            }
            var result = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.TuornamentID == tournamentID && (tm.MemberStatus == tournamentMemberStatus)&&(tm.Golfer.FirstName.Trim().ToLower().Contains(searchKey)|| tm.Golfer.LastName.Trim().ToLower().Contains(searchKey))).Skip(startIndex).Take(Const.PageSize).ToList();
            List<TournamentMemberResponse> tournamentMemberResponses = new List<TournamentMemberResponse>();
            foreach (var i in result)
            {
                tournamentMemberResponses.Add(GetTournamentMemberResponse(i));
            }
            return tournamentMemberResponses;
        }
        public List<TournamentMemberResponse> SearchJoinRequest(Guid currentID, Guid tournamentID, string searchKey, int startIndex)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID)
            {
                throw new ForbiddenException("Not allow!");
            }
            var tournamentMembers = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.TuornamentID == tournamentID && (tm.MemberStatus == TournamentMemberStatus.Requested)).ToList();
            var result = tournamentMembers.FindAll(tm => tm.Golfer.GetFullName().Trim().ToLower().Contains(searchKey.Trim().ToLower())).Skip(startIndex).Take(Const.PageSize);
            List<TournamentMemberResponse> tournamentMemberResponses = new List<TournamentMemberResponse>();
            foreach (var i in result)
            {
                tournamentMemberResponses.Add(GetTournamentMemberResponse(i));
            }
            return tournamentMemberResponses;
        }
        public List<TournamentMemberResponse> GetRequestedMember(Guid currentID, Guid tournamentID, int startIndex)
        {
            var tournament = _tournamentRepository.Get(tournamentID);
            if (tournament == null)
            {
                throw new NotFoundException("Tournament not found!");
            }
            if (tournament.OwnerID != currentID)
            {
                throw new ForbiddenException("Not allow!");
            }
            var tournamentMembers = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.TuornamentID == tournamentID && tm.MemberStatus == TournamentMemberStatus.Requested).Skip(startIndex).Take(Const.PageSize);
            List<TournamentMemberResponse> tournamentMemberResponses = new List<TournamentMemberResponse>();
            foreach (var i in tournamentMembers)
            {
                tournamentMemberResponses.Add(GetTournamentMemberResponse(i));
            }
            return tournamentMemberResponses;
        }
        public TournamentMemberResponse GetTournamentMemberResponse(TournamentMember tournament)
        {
            TournamentMemberResponse tmp = new TournamentMemberResponse();
            var golfer = _mapper.Map<MinimizedGolfer>(tournament.Golfer);
            tmp.MemberID = tournament.ID;
            tmp.Golfer = golfer;
            tmp.MemberStatus = tournament.MemberStatus;
            return tmp;
        }
        public TournamentMemberResponseDetail GetTournamentMemberResponseDetail(Guid memberID)
        {
            TournamentMemberResponseDetail tournamentMemberResponse = new TournamentMemberResponseDetail();
            var member = _tournamentMemberRepository.Get(memberID);
            var profile = _profileRepository.Get(member.GolferID);
            var golfer = _golferRepository.Get(member.GolferID);
            var scorecards = _scorecardRepository.GetScorecardsByDateRangeFilter(member.GolferID, DateRangeFilter.All, new DateTime(), new DateTime());
            tournamentMemberResponse.ID = golfer.Id;
            tournamentMemberResponse.Image = golfer.Avatar;
            tournamentMemberResponse.Name = golfer.GetFullName();
            tournamentMemberResponse.PhoneNumber = golfer.PhoneNumber;
            tournamentMemberResponse.Email = golfer.Email;
            tournamentMemberResponse.Handicap = golfer.Handicap;
            tournamentMemberResponse.TotalMatch = scorecards.Count();
            tournamentMemberResponse.Gender = profile.Gender;
            tournamentMemberResponse.AvgScore = scorecards.Count() == 0 ? 0 : scorecards.Average(sc => sc.Grosses);
            if (profile.Birthday != null)
            {
                var tmp = (DateTime)profile.Birthday;
                if (DateTime.Compare(DateTime.Now, tmp) < 0)
                {
                    tournamentMemberResponse.Age = 0;
                }
                else
                {
                    if (tmp.Year == DateTime.Now.Year)
                    {
                        tournamentMemberResponse.Age = 1;
                    }
                    else
                        tournamentMemberResponse.Age = DateTime.Now.AddYears(1).AddYears((-1) * tmp.Year).AddMonths((-1) * tmp.Month).AddDays((-1) * tmp.Day).Year;
                }
            }
            var bestGross = scorecards.Count() == 0 ? 0 : scorecards.Min(s => s.Grosses);
            var bestScorecard = scorecards.Count() == 0 ? null : scorecards.Find(s => s.Grosses == bestGross);
            tournamentMemberResponse.BestMatch = scorecards.Count() == 0 ? null : _scorecardService.GetMinimizedScorecard(bestScorecard);
            tournamentMemberResponse.Address = profile.Address;
            return tournamentMemberResponse;
        }
        public TournamentResponse GetTournammentRespone(Guid currentID, Tournament tournament)
        {
            TournamentResponse tournamentResponse = new TournamentResponse();
            _mapper.Map(tournament, tournamentResponse);
            if (tournament.OwnerID == currentID)
            {
                tournamentResponse.IsOwner = true;
            }
            var tournamentMember = _tournamentMemberRepository.FindIncludeGolfer(tm => tm.GolferID == currentID && tm.TuornamentID == tournament.ID);
            if (tournamentMember.Count() <= 0)
            {
                tournamentResponse.MemberStatus = MemberStatusResponse.None;
            }
            else
            {
                switch (tournamentMember.First().MemberStatus)
                {
                    case TournamentMemberStatus.Requested:
                        {
                            tournamentResponse.MemberStatus = MemberStatusResponse.Requested;
                            break;
                        }
                    case TournamentMemberStatus.Joined:
                        {
                            tournamentResponse.MemberStatus = MemberStatusResponse.Joined;
                            break;
                        }
                    case TournamentMemberStatus.Rejected:
                        {
                            tournamentResponse.MemberStatus = MemberStatusResponse.Rejected;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            tournamentResponse.Status = tournament.Status;
            if (DateTime.Compare(DateTime.Now, tournament.DateTime) < 0)
            {
                tournamentResponse.IsFinish = false;
            }
            else
            {
                tournamentResponse.IsFinish = true;
            }
            return tournamentResponse;
        }

        public List<TournamentResponse> GetTournammentRespones(Guid currentID, List<Tournament> tournaments)
        {
            List<TournamentResponse> tournamentResponses = new List<TournamentResponse>();
            foreach (var i in tournaments)
            {
                tournamentResponses.Add(this.GetTournammentRespone(currentID, i));
            }
            return tournamentResponses;
        }
        /// <summary>
        /// Tạo nhóm cho giải đấu
        /// /// </summary>
        /// <param name="currentID">địanh danh người dùng hiện thời</param>
        /// <param name="tournament">Giải đấu</param>
        /// <returns></returns>
        
    }
}
