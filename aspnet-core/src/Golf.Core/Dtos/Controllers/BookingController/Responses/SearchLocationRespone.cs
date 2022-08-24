using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class SearchLocationRespone
    {
        public SearchLocationRespone() { }
        public SearchLocationRespone(Guid id, string name, string adress, string country)
        {
            ID = id;
            Name = name;
            Address = adress;
            Country = country;
        }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}
