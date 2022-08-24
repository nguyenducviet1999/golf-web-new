using Golf.Domain.Common;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
   public class BookingResponse
    {
      
        public Guid ID { get; set; }// id của địa điểm
        public string Name { get; set; }//Tên địa điểm
        public bool IsFavorite { get; set; }// Được yêu thích hay không
        public bool IsPromotion { get; set; }//có khuyến mại hay không
        public TimeSpan FirstTeeTime { get; set; }// Teetime đầu tiên
        public TimeSpan LastTeeTime { get; set; }//TeeTime cuối cùng
        public double Price { get; set; }// Giá thực tế
        public double Promotion { get; set; }//Phần trăm khuyến mại
        public double MembershipPromotion { get; set; }//Phần trăm khuyến mại thành viên
        public double ReviewPoint { get; set; }// điểm đánh giá
        public string Cover { get; set; }// Ảnh bìa
        public string Address { get; set; }// Địa chị
        public int TotalReview { get; set; } = 0;// số lượng review
        public string PaymentType { get; set; }// hình thức thanh toán
        public List<CourseInformation> MoreInformations { get; set; }
        public GPSAddress GPSAddress { get; set; }//vị trí
        public string Distance { get; set; }// Khoảng cách
        

        //public string HeadOffice { get; set; }
    }
}
