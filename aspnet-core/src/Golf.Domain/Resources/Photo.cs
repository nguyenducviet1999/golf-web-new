using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using Golf.Domain.Base;
using Golf.Domain.Shared.Resources;

namespace Golf.Domain.Resources
{
    public class Photo
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? CreatedBy { get; set; }
        public PhotoType Type { get; set; }
    }
}
