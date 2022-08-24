using Golf.Core.Common.Golfer;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Golf.Core.Dtos.Controllers.AuthController.Responses
{
    public class DefaultAuthResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class AdminAuthResponse
    {
        public MinimizedGolfer Golfer { get; set; }
        public string Roles { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class OdooUserDto
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("partner_id")]
        public int PartnerID { get; set; }
        [JsonProperty("website_id")]
        public int WebsiteID { get; set; }
        [JsonProperty("groups")]
        public List<OdooGroupResponse> Groups { get; set; }
    }
    public class OdooGroupResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("category_id")]
        public List<dynamic> PartnerID { get; set; }
    }
    public class LoginOdooResponse
    {
        

        [JsonProperty("user")]
        public OdooUserDto User { get; set; }
    }
    public class SignUpOdooResponse
    {

        [JsonProperty("user")]
        public OdooUserDto User { get; set; }
    }
}
