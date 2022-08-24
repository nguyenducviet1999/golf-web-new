using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.MembershipController.Responses
{
    public class OdooMembershipResponse
    {
        public int ID;
        public string Name;
        public string MembershipDateFrom;
        public string MembershipDateTo;
        public double lListPrice;
        public string DescriptionSale;
    }
    public class OdooMembershipResponseDto
    {
        [JsonProperty("id")]
        public int ID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("membership_date_from")]
        public string MembershipDateFrom;
        [JsonProperty("membership_date_to")]
        public string MembershipDateTo;
        [JsonProperty("list_price")]
        public double lListPrice;
        [JsonProperty("description_sale")]
        public string DescriptionSale;
    }
    public class OdooMyMembershipResponse
    {
        public int ID;
        public string Name;
        public string MembershipDateFrom;
        public string MembershipDateTo;
        public double lListPrice;
        public string DescriptionSale;
        public string Date;
        public string DateCancel;
        public string State;
    }
    public class OdooMyMembershipResponseDto
    {
        [JsonProperty("membership_id")]
        public int ID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("date_from")]
        public string MembershipDateFrom;
        [JsonProperty("date_to")]
        public string MembershipDateTo;
        [JsonProperty("member_price")]
        public double lListPrice;
        [JsonProperty("description_sale")]
        public string DescriptionSale;
        [JsonProperty("date")]
        public string Date;
        [JsonProperty("date_cancel")]
        public string DateCancel;
        [JsonProperty("state")]
        public string State;
    }
}
