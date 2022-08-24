using Golf.Domain.Shared;
using Golf.Domain.Shared.Groups;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Groups
{
    public class GroupResponse
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int NumberMember { get; set; }
        public string Description { get; set; }//Mô tả thêm
        public string Cover { get; set; }
        public GroupType Type { get; set; }
        public MemberStatus MemberStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}