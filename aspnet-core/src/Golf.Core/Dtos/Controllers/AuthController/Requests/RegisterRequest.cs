using System;
using System.Collections.Generic;
using System.Text;
using Golf.Core.Dtos.Request;

namespace Golf.Core.Dtos.Controllers.AuthController.Requests
{
    public class RegisterRequest : IRequest
    {
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Handicap { get; set; }
        public string Password { get; set; }
        public int? CountryID { get; set; }
        public int? StateID { get; set; }
        public List<string> Roles { get; set; }

        public override bool Validate()
        {
            if (!IsNumber(this.PhoneNumber))
            {
                return false;
            }
            if (this.Password.Length < 8)
            {
                return false;
            }
            return true;
        }

    }
}
