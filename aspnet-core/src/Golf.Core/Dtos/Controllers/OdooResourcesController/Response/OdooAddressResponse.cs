using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.OdooResourcesController.Response
{
    public class OdooAddressResponse
    {
        public int ID;
        public string Name;
        public string Code;
        
    }
    public class OdooAddressResponseDto
    {
        [JsonProperty("id")]
        public int ID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("code")]
        public string Code;
    }
}
