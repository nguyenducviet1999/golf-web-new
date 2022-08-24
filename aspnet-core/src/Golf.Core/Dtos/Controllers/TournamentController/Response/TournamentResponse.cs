using Golf.Domain.Shared.Tuanament;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TournamentController.Response
{
   public class TournamentResponse
    {
        public Guid ID { get; set; }
        public string OrganizationalUnitName { get; set; }//Tên đơn vị tố chức giải đaus
        public string TournamentName { get; set; }//tên giải đáu
        public string Venue { get; set; }//địa điểm tổ chức
        public string ContactName { get; set; }//tên liên hệ
        public string ContactPhoneNumber { get; set; }//số điện thoại liên hệ
        public string ContactEmail { get; set; }//số điện thoại liên hệ
        public string Description { get; set; }//Mô tả
        public double Fees { get; set; }//Lệ phí
        public bool IsFinish { get; set; }//Lệ phí
        public Guid? GroupID { get; set; }//Lệ phí
        public bool IsOwner { get; set; } = false;//Lệ phí
        //public int MaxMember { get; set; }//Sô lượng tôi đa
        public MemberStatusResponse MemberStatus { get; set; }//trạng thái tham gia sự kiện
        public TournamentStatus Status { get; set; }//trạng thái tham gia sự kiện
        public DateTime RegistrationDeadline { get; set; }//thời hạn đăng kí tham gia
        public DateTime DateTime { get; set; }//thời điểm diễn ra
     
    }
}
