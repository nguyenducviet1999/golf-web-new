using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.OdooResourcesController.Requests
{
    public class OdooStateRequestDto
    {
        [JsonProperty("fields")]
        public List<string> Fields { get; set; }
        [JsonProperty("domain")]
        public string Domain { get; set; }
        //[JsonProperty("offset")]
        //public string Offset { get; set; }
    }
    public class OdooStateRequest
    {
        public OdooStateRequest(int iD)
        {
            ID = iD;
        }
        public int ID { get; set; }
        public OdooStateRequestDto GetListStateByCountryIDRequest()
        {
            return new OdooStateRequestDto()
            {
                Fields = new List<string>() { "id", "name", "code" },
                Domain = "[('country_id', '=', " + ID + ")]"
            };
        }
        public OdooStateRequestDto GetStateByIDRequest()
        {
            return new OdooStateRequestDto()
            {
                Fields = new List<string>() { "id", "name", "code" },
                Domain = "[('id', '=', " + ID + ")]"
            };
        }
    }
}
