using Golf.Domain.Base;
using Golf.Domain.GolferData;
using Golf.Domain.Post;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Golf.Domain.Report
{
   public class Report: IEntityBase
    {
        public string Content { get; set; }
        public string Image { get; set; }
        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }
        public virtual Golfer Owner { get; set; }
        [Column(TypeName = "jsonb")]
        public ReferenceObject ReferenceObject { get; set; }
    }
}
