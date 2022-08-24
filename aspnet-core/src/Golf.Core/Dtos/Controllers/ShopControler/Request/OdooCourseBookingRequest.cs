using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooCourseBookingRequestDto
    {
        [JsonProperty("parner_id")]
        public long ParnerID { get; set; }
        [JsonProperty("amount")]
        public double Amount { get; set; } 
        [JsonProperty("user_id")]
        public long? GSAID { get; set; }
    }
    public class OdooCourseBookingRequest
    {
        public double Amount { get; set; } 
        public long? GSAID { get; set; }
        public OdooCourseBookingRequestDto ToOdooCourseBookingRequestDto(long parnerID)
        {
            return new OdooCourseBookingRequestDto()
            {
                Amount = Amount,
                GSAID = GSAID,
                ParnerID = parnerID
            };
        }
    }
}
