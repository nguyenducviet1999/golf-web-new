using Golf.Core.Common.Odoo.OdooResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooProductResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("list_price")]
        public double Price { get; set; }
        [JsonProperty("image_128")]
        public dynamic Image { get; set; }
        [JsonProperty("public_categ_ids")]
        public List<int> CategoryID { get; set; }
        [JsonProperty("is_published")]
        public bool IsPublished { get; set; }
    } 
    public class OdooProductResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public List<int> CategoryID { get; set; }
        public bool IsPublished { get; set; }
    }
}
