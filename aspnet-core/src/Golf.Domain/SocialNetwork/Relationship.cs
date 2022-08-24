using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Relationship;

namespace Golf.Domain.SocialNetwork
{
    public class Relationship : IEntityBase
    {
        [ForeignKey("Golfer")]
        public Guid GolferID { get; set; }
        public virtual Golfer Golfer { get; set; }
        [ForeignKey("Friend")]
        public Guid FriendID { get; set; }
        public virtual Golfer Friend { get; set; }

        public RelationshipStatus Status { get; set; }
    }
}