using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Scorecard;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Scorecard
{
    public class ScorecardVote: IEntityBase
    {
        [ForeignKey("Owner")]
        public Guid GolferID { get; set; }//định danh người vote
        public virtual Golfer Golfer { get; set; }
        [ForeignKey("Owner")]
        public Guid ScorecardID { get; set; }//định danh bảng điểm
        public virtual Scorecard Scorecard { get; set; }
        public ScorecardVoteType Type { get; set; } //loại vote(xác nhận, báo sai)
    }
}
