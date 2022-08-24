using Golf.Core.Common.Golfer;
using Golf.Domain.Shared.Tuanament;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AdminController.Tournament
{
   public class TournamentAdminResponse
    {
        public Guid ID { get; set; }
        public Guid OwnerID { get; set; }//định daanh người tạo
        public virtual MinimizedGolfer Owner { get; set; }//người tạo giải đấu
        public DateTime DateTime { get; set; }//thời điểm diễn ra
        public string ContactName { get; set; }//tên liên hệ
        public string ContactPhoneNumber { get; set; }//số điện thoại liên hệ
        public string ContactEmail { get; set; }//số điện thoại liên hệ
        public string OrganizationalUnitName { get; set; }//Tên đơn vị tố chức giải đaus
        public string TournamentName { get; set; }//tên giải đáu
        public string Venue { get; set; }//địa điểm tổ chức
        public string Description { get; set; }//Mô tả
        public double Fees { get; set; }//Lệ phí
        public Guid? GroupID { get; set; }//Lệ phí
        public TournamentStatus Status { get; set; }//trạng thái giải đấu
        public DateTime RegistrationDeadline { get; set; }//thời hạn đăng kí tham gia
    }
}
