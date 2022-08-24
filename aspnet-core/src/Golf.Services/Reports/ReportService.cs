using Golf.Core.Dtos.Controllers.AdminController.Report.Request;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Domain.Post;
using Golf.Domain.Report;
using Golf.Domain.Resources;
using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Golfer;
using Golf.Domain.Shared.Post;
using Golf.Domain.Shared.Resources;
using Golf.Domain.Shared.Scorecard;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Notifications;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Reports
{
    public class ReportService
    {
        private readonly ReportRepository _reportRepository;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly PhotoService _photoService;
        private readonly NotificationService _notificationService;
        private readonly UserManager<Golfer> _golferManager;
        private readonly PostVoteService _postVoteService;
        private readonly ScorecardVoteRepository _scorecardVoteRepository;
        public ReportService(ScorecardVoteRepository scorecardVoteRepository, PostVoteService postVoteService, UserManager<Golfer> golferManager, NotificationService notificationService, PhotoService photoService, DatabaseTransaction databaseTransaction, ReportRepository reportRepository, ScorecardRepository scorecardRepository)
        {
            _scorecardVoteRepository = scorecardVoteRepository;
            _postVoteService = postVoteService;
            _golferManager = golferManager;
            _notificationService = notificationService;
            _photoService = photoService;
            _databaseTransaction = databaseTransaction;
            _reportRepository = reportRepository;
            _scorecardRepository = scorecardRepository;
        }
        public async Task<bool> Add(Guid currentUserID, ReportRequest reportRequest)
        {
            Photo photo=null;
            if(reportRequest.PhotoFile!=null)
            {
                 photo = await _photoService.SavePhoto(currentUserID, reportRequest.PhotoFile.photo, PhotoType.Group);
            }    
            _databaseTransaction.BeginTransaction();
            try
            {
                switch (reportRequest.ReferenceObject.Type)
                {
                    case ReferenceObjectType.Scorecard:
                        {
                            var scorecard = _scorecardRepository.Get(reportRequest.ReferenceObject.ID);
                            if (scorecard == null)
                            {
                                throw new NotFoundException("Not found scorecard");
                            }
                            var scorecardVote = _scorecardVoteRepository.Find(scv => scv.GolferID == currentUserID && scv.ScorecardID == scorecard.ID).FirstOrDefault();
                            if (scorecard != null&&scorecard.Type==ScorecardType.Posted)
                            {
                                if (scorecard.OwnerID == currentUserID)
                                {
                                    return false;
                                }
                                if (scorecardVote == null )
                                {
                                    _scorecardVoteRepository.SafeAdd(new ScorecardVote() { GolferID = currentUserID, ScorecardID = scorecard.ID, Type = ScorecardVoteType.ConfirmIncorrect });
                                    _notificationService.NotificationConfirmIncorrectScorecard(currentUserID, scorecard.OwnerID, scorecard.ID);
                                    Report report = new Report();
                                    report.Content = reportRequest.Content;
                                    report.OwnerID = currentUserID;
                                    report.ReferenceObject = reportRequest.ReferenceObject;
                                    report.Image = photo == null ? null : photo.Name;
                                    _reportRepository.SafeAdd(report);
                                }
                                else
                                {
                                   return false;
                                }
                            }
                            await _databaseTransaction.Commit();
                            //notification
                            var admins = await _golferManager.GetUsersInRoleAsync(RoleNormalizedName.SystemAdmin);
                            foreach (var i in admins)
                            {
                               await _notificationService.NotificationReportScorecard(currentUserID, i.Id, scorecard.ID);
                            }
                            break;
                        }
                    default:
                        {
                            throw new BadRequestException("Invalid object type");
                        }
                }
                return true;
            }
            catch (Exception e)
            {
                if(photo!=null)
                {
                    _photoService.DeletePhoto(photo.Name);
                }    
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
    }
}
