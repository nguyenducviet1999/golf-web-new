using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.MembershipController.Requests
{
   public class BuyMembershipRequestDto
    {
        public BuyMembershipRequestDto(int productID)
        {
            ProductID = productID;
        }
        [JsonProperty("product_id")]
        public int ProductID;
    }
}
