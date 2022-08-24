using Golf.Core.Dtos.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AuthController.Requests
{
    public class SignUpRequest : IRequest
    {
        public string Email { get; set; }//email
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Handicap { get; set; }
        public string Password { get; set; }
        public int? CountryID { get; set; }
        public int? StateID { get; set; }
       // public List<string> Roles { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public override bool Validate()
        {
            return true;
        }
        public string GetFulName()
        {
            return FirstName + " " + LastName;
        }
    }
    public class SignUpOdooDto
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("db")]
        public string Db { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; } 
        [JsonProperty("country_id")]
        public int? CountryID { get; set; }
        [JsonProperty("state_id")]
        public int? StateID { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("street2")]
        public string Street2 { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("customer_types")]
        public List<int> CustomerTypes { get; set; }
    }
}
