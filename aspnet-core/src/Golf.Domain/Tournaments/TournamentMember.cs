using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Tuanament;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Tournaments
{
    public class TournamentMember: IEntityBase
    {
        [ForeignKey("Golfer")]
        public Guid GolferID { get; set; }//định daanh người tạo
        public Golfer Golfer { get; set; }
        [ForeignKey("Tuanament")]
        public Guid TuornamentID { get; set; }//định daanh người tạo
        public Tournament Tuanament { get; set; }
        public TournamentMemberStatus MemberStatus { get; set; }
    }
}
