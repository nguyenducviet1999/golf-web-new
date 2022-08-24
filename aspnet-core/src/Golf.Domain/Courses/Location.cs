using Golf.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

using Golf.Domain.Common;
namespace Golf.Domain.Courses
{
    public class Location : IEntityBase
    {
        public Guid MainVersionID { get; set; }
        public Guid OwnerID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address { get; set; }
        public bool IsConfirmed { get; set; }
        [Column(TypeName = "jsonb")]
        public GPSAddress GPSAddress { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string HeadOffice { get; set; }
        public int Version { get; set; }
        public string Email { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
