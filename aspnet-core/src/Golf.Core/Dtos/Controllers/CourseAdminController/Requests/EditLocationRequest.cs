using System;
using Golf.Core.Dtos.Request;
using Golf.Domain.Common;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Requests
{
    public class EditLocationRequest : IRequest
    {
        public Guid LocationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string HeadOffice { get; set; }
        public  GPSAddress GPSAddress{ get; set; }

        public override bool Validate()
        {
            return true;
        }
    }
}