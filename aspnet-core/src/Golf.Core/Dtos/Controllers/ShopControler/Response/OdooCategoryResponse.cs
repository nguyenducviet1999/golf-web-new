using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
   public class OdooCategoryResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("sequence")]
        public int Sequence { get; set; }
    } 
    public class OdooCategoryResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
    }
}
