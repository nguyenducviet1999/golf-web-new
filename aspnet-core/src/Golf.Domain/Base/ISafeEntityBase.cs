using System;
using System.ComponentModel.DataAnnotations;

namespace Golf.Domain.Base
{
    public abstract class ISafeEntityBase
    {
       public ISafeEntityBase()
        {
            this.ID = Guid.NewGuid();
        }
        [Key]
        public Guid ID { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public Guid? DeleteBy { get; set; }
    }
}