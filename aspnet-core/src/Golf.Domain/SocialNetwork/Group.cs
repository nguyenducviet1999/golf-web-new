using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Groups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.SocialNetwork
{
    public class Group : IEntityBase
    {
        //public Guid CreatorID { get; set; }

        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }
        public virtual Golfer Owner { get; set; }
        public string Name { get; set; }//tên nhóm
        public string Description { get; set; }//Mô tả thêm
        public GroupType Type { get; set; } //Phạm vi nhóm(kín, công  khai)
        //public int GroupStatus { get; set; }
        public string Cover { get; set; }//ảnh bìa
        public DateTime? DeleteDate { get; set; }
    }
}
