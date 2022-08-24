using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.SocialNetwork
{
    public class GroupMember : IEntityBase
    {

        [ForeignKey("Golfer")]
        public Guid GolferID { get; set; }
        public virtual Golfer Golfer { get; set; }
        public int GroupRole { get; set; }
        [ForeignKey("Group")]
        public Guid GroupID { get; set; }
        public virtual Group Group { get; set; }
        public MemberStatus Status { get; set; }
    }
}
