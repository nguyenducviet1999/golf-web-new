using Golf.Core.Dtos.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    public class OdooProfileRequest : IRequest
    {
        public string Email { get; set; }//email
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // public string Country { get; set; }
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
        public OdooProfileDto getOdooProfileDto()
        {
            OdooProfileDto dto = new OdooProfileDto();
            dto.Login = Email;
            dto.Name = GetFulName();
            dto.Street = Street;
            dto.Street2 = Street;
            dto.Zip = Zip;
            return dto;
        }
    }
    public class OdooProfileDto
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("country_id")]
        public int? CountryID { get; set; } = null;
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("street2")]
        public string Street2 { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }

}
