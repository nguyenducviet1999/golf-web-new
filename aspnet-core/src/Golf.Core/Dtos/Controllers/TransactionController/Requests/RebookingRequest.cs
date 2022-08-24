using Golf.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.TransactionController.Requests
{
    public class RebookingRequest
    {

        public int NumberOfGolfer { set; get; }
        public Guid ProductID { set; get; }
        public string PromotionCode { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public List<int> MoreRequests { get; set; }
    }
}
