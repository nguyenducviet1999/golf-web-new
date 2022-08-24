using Golf.Domain.Base;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Golf.Domain.Bookings
{
    public class Transaction : IEntityBase
    {
        [ForeignKey("Golfer")]
        public Guid GolferID { get; set; }
        public virtual Golfer Golfer { get; set; }
        [ForeignKey("Product")]
        public Guid ProductID { set; get; }//định danh giờ chơi(sản phẩm)
        public virtual Product Product { get; set; }
        [ForeignKey("Salesman")]
        public Guid? SalesmanID { set; get; }//định danh giờ chơi(sản phẩm)
        public virtual Golfer Salesman { get; set; }
        public int NumberOfGolfer { set; get; }//số lượng người chơi
        public TransactionStatus Status { get; set; }//Trạng thái giao dịch
        public double Amount { get; set; }// giá tiền 
        public string PromotionCode { get; set; }
        [Column(TypeName = "jsonb")]
        public ContactInfo ContactInfo { get; set; }
        public string Description { get; set; }
        public List<int> MoreRequests { get; set; }

    }
}
