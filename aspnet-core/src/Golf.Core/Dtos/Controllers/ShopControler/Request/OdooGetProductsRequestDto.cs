using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooGetProductsRequestDto
    {
        [JsonProperty("fields")]
        public List<string> Fields;
        [JsonProperty("domain")]
        public string Domain;
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("offset")]
        public int Offset;
        public OdooGetProductsRequestDto(int filter, int startIndex)
        {
            Fields = new List<string> { "id", "name", "list_price", "image_128", "public_categ_ids", "is_published" };
            Domain = "[('is_published', '=', True)" + (filter < 0 ? "" : ", ('public_categ_ids', 'in', [" + filter + "])") + "]";
            Limit = Const.PageSize;
            Offset = startIndex;
        }
    }
}
