using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AdminController.Account.Request
{
   public class AddLocationAdminRequest
    {
        public string Email { get; set; }//email
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public int? CountryID { get; set; }
        // public List<string> Roles { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public List<Guid> LocationIDs { get; set; }
    }
}
