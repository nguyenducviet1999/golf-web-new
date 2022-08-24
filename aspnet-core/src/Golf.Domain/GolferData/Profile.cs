using Golf.Domain.Base;
using System;

using Golf.Domain.Shared.Golfer.Information;

namespace Golf.Domain.GolferData
{
    public class Profile : IEntityBase
    {
        public DateTime? Birthday { get; set; } = null;
        public Gender Gender { get; set; }
        public int? CountryID { get; set; } = null;
        public int? StateID { get; set; } = null;
        public string Address { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Workfield { get; set; }
        public string Website { get; set; }
        public string Workplace { get; set; }
        public ClothesSizeType ClothesSizeType { get; set; }
        public ShoesSizeType ShoesSizeType { get; set; }
        public int ShoesSize { get; set; }
        public int ShirtSize { get; set; }
        public int PantsSize { get; set; }

       // public int FootLength { get; set; } = 0;

        public PreferredHand PreferredHand { get; set; }
        public string Quote { get; set; }

    }
}