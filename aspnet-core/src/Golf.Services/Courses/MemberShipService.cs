using Golf.Core.Common.Golfer;
using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Course;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Golf.Services.Courses
{
    public class MemberShipService
    {
        private readonly MemberShipRepository _memberShipRepository;
        private readonly GolferRepository _golferRepository;
        private readonly GolferService  _golferService;
        private readonly CourseRepository _courseRepository;
        public MemberShipService(GolferService golferService,MemberShipRepository courseMemberShipRepository, GolferRepository golferRepository, CourseRepository courseRepository)
        {
            _golferService = golferService;
            _memberShipRepository = courseMemberShipRepository;
            _courseRepository = courseRepository;
            _golferRepository = golferRepository;
        }

        public bool IsMemberShip(Guid golferID)
        {
            var membership = _memberShipRepository.Find(mb=>mb.GolferID == golferID && mb.Status == CourseMemberShipStatus.IsMemberShip);
            if (membership.Count() == 0)
                return false;
            return true;
        }

        public bool AddMemberShip(Guid golferID)
        {

            var memberShip = _memberShipRepository.Find(cms =>cms.GolferID == golferID);
            if (memberShip.Count() > 0)
            {
                throw new BadRequestException("MemberShip is exit!");
            }
            _memberShipRepository.Add(new MemberShip() { GolferID = golferID, Status = CourseMemberShipStatus.IsMemberShip });
            return true;
        }
        public bool ConfirmMemberShip(Guid golferID)
        {

            var memberShip = _memberShipRepository.Find(m=>m.GolferID==golferID).FirstOrDefault();
            if (memberShip==null)
            {
                throw new BadRequestException("Not found MemberShip!");
            }
            if(memberShip.Status==CourseMemberShipStatus.Request)
            {
                memberShip.Status = CourseMemberShipStatus.IsMemberShip;
                _memberShipRepository.UpdateEntity(memberShip);
                return true;
            }    
            else
            {
                throw new BadRequestException("Golfer is MemberShip");
            }    
        }
        public bool SendMemberShipRequest(Guid golferID)
        {
           
            var memberShip = _memberShipRepository.Find(cms =>  cms.GolferID == golferID);
            if (memberShip.Count() > 0)
                throw new BadRequestException("MemberShip is exit!");
            _memberShipRepository.Add(new MemberShip() { GolferID = golferID, Status = CourseMemberShipStatus.Request });
            return true;
        }
        public List<MinimizedGolfer> GetMemberShipRequest(int startIndex)
        {
           
            var memberShips = _memberShipRepository.GetAll().Skip(startIndex).Take(Const.PageSize).ToList();
            List<MinimizedGolfer> golfers = new List<MinimizedGolfer>();
            foreach(var i in memberShips)
            {
                golfers.Add(_golferService.GetMinimizedGolfer(i.GolferID));
            }    
            return golfers;
          
        }

    }
}
