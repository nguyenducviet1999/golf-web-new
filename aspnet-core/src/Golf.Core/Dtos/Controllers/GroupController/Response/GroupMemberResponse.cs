using Golf.Core.Common.Golfer;
using Golf.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GroupController.Response
{
    public class GroupMemberResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public int TotalMatches { get; set; }
        public string Relationship { get; set; }
        public MemberStatus MemberType  { get; set; }
    }
}
