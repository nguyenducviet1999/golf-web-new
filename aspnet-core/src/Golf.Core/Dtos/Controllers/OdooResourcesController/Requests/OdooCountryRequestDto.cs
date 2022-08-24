using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.OdooResourcesController.Requests
{
    public class OdooCountryRequestDto
    {
        public OdooCountryRequestDto()//lấy danh sách quóc gia
        {
            Fields = new List<string> { "id", "name", "code" };
            //Limit = Const.PageAmount;
            //Offset = startIndex;
        } 
        public OdooCountryRequestDto(int id)//lấy quốc gia theo id
        {
            Fields = new List<string> { "id", "name", "code" };
            Domain = "[('id', '=', " + id + ")]";
        }

        [JsonProperty("fields")]
        public List<string> Fields;
        [JsonProperty("domain")]
        public string Domain;
        //[JsonProperty("limit")]
        //public int Limit;
        //[JsonProperty("offset")]
        //public int Offset;
    }
}