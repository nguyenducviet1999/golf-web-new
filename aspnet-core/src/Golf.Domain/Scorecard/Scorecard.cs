using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Golf.Domain.Base;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Scorecard;

namespace Golf.Domain.Scorecard
{
    public class Scorecard : IEntityBase
    {
        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }//định danh người sở hữu
        public virtual Golfer Owner { get; set; }// người sở hữu
        [ForeignKey("Course")]
        public Guid CourseID { get; set; }//định danh sân
        public virtual Course Course { get; set; }//sân
        public double HandicapBefore { get; set; }//Điểm chấp trước khi chơi
        public double? HandicapAfter { get; set; }//Điểm chấp sau khi chơi
        public double CourseHandicapBefore { get; set; }//Điểm chấp sân trước khi chơi
        public double? CourseHandicapAfter { get; set; }//Điểm chấp sân sâu khi chơi
        public double HandicapDifferential { get; set; }//Điểm chấp chênh lệch
        public double System36Handicap { get; set; }//
        [Column(TypeName = "jsonb")]
        public Tee Tee { get; set; }//loại chơi
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }//Ngày chơi
        public DateTime? PostDate { get; set; }//Ngày đăng
        [Column(TypeName = "jsonb")]
        public List<Hole> Holes { get; set; } = new List<Hole>();//Điểm từng hố
        [Column(TypeName = "jsonb")]
        public List<double> ParsAverage { get; set; } //điểm par trung bình
        public int Grosses { get; set; }//Điểm 
        public double RealGrosses { get; set; }//Điểm thực
        [Column(TypeName = "jsonb")]
        public Achievements Achievements { get; set; }//Thống kê
        public bool IsConfirmed { get; set; } = false;
        //public bool IsPending { get; set; }
        //public bool IsVerified { get; set; }
        public ScorecardInputType InputType { get; set; }
        public ScorecardType Type { get; set; }
        public int BestHole { get; set; }
        public int AmountHoles { get; set; }
    }
}
