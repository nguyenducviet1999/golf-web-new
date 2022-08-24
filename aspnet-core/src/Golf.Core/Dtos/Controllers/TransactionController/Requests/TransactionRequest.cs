using Golf.Domain.Bookings;
using Golf.Domain.Courses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TransactionController.Requests
{
    public class TransactionRequest
    {
        public Guid GolferID { get; set; }
        public Guid ProductID { set; get; }
        public int NumberOfGolfer { set; get; }
        public string PromotionCode { get; set; } 
        public ContactInfo ContactInfo { get; set; }
        public List<int> MoreRequests { get; set; }

       
    }
}
