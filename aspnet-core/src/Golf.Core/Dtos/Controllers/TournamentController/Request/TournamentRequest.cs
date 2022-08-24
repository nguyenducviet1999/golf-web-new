using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TournamentController.Request
{
    public class TournamentRequest
    {
        public DateTime Date { get; set; }//thời điểm diễn ra
        public DateTime Time { get; set; }//thời điểm diễn ra
        public string ContactName { get; set; }//tên liên hệ
        public string ContactPhoneNumber { get; set; }//số điện thoại liên hệ
        public string ContactEmail { get; set; }//số điện thoại liên hệ
        public string OrganizationalUnitName { get; set; }//Tên đơn vị tố chức giải đaus
        public string TournamentName { get; set; }//tên giải đáu
        public string Venue { get; set; }//địa điểm tổ chức
        public string Description { get; set; }//Mô tả
        public double Fees { get; set; }//Lệ phí
        //public bool AddGroup { get; set; } = false;//Lệ phí
        //public int MaxMember { get; set; }//Sô lượng tôi đa
        public DateTime RegistrationDeadline { get; set; }//thời hạn đăng kí tham gia

        public bool Validate()
        {
            if (Date==null||DateTime.Compare(DateTime.Now,Date)<0)
                return false;
            if (Time == null)
                return false;
            if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;if (ContactName == "" || ContactName == null)
                return false;
            return true;
        }
    }
}
