using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooAddressRequestDto 
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("phone")]
        public string Phone;
        [JsonProperty("email")]
        public string Email;
        [JsonProperty("street")]
        public string Street;
        [JsonProperty("city")]
        public string City;
        [JsonProperty("country_id")]
        public int CountryID;
        [JsonProperty("state_id")]
        public int StateID;
        [JsonProperty("partner_id")]
        public int ID; 
        [JsonProperty("is_default_shipping")]
        public bool IsDefaultShipping;
     
    } 
    public class OdooAddressRequest
    {
        public string Name;
        public string Phone;
        public string Email;
        public string Street;
        public string City;
        public int CountryID;
        public int StateID;
        public bool IsDefaultShipping;
    }

    public class OdooGetMyAddressRequestDto
    {
        [JsonProperty("fields")]
        public List<string> Fields;
        [JsonProperty("domain")]
        public string Domain;
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("offset")]
        public int Offset;
        public OdooGetMyAddressRequestDto(int partnerID, int startIndex)
        {
            Fields = new List<string> { "id", "name", "street", "street2", "phone", "email", "city", "state_id", "country_id", "type", "zip", "is_default_shipping" };
            Domain = "[('active', '=', True),('commercial_partner_id.id', '=', '"+partnerID+"')]";//thiếu chờ odoo
            Limit = Const.PageSize;
            Offset = startIndex;
        }
    }
    public class OdooChooseAddressDto
    {
        [JsonProperty("partner_id")]
        public int PartnerID;
        public OdooChooseAddressDto(int partnerID)
        {
            PartnerID = partnerID;
        }
    }
}
