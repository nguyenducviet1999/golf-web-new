using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.MembershipController.Requests
{
    public class OdooMembershipRequestDto
    {
        public OdooMembershipRequestDto(int startIndex)
        {
            Offset = startIndex;
            Limit = Const.PageSize;

        }
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("offset")]
        public int Offset;
    }
}
