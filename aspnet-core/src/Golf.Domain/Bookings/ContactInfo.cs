using System.Collections.Generic;

namespace Golf.Domain.Bookings
{
    public class ContactInfo
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public List<string> ListPlayer { get; set; }
    }
}
