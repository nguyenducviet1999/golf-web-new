using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
   public class OdooWishListRequestDto
    {
        //[JsonProperty("fields")]
        //public List<string> Fields;
        [JsonProperty("domain")]
        public string Domain;
        [JsonProperty("limit")]
        public int? Limit;
        [JsonProperty("offset")]
        public int Offset;
        public OdooWishListRequestDto(int startIndex,int? pageSize,string searchKey)
        {
            string filter = "";
            if (searchKey != null && searchKey != "")
            {
                searchKey = searchKey.Trim();
                filter = "('product_id.name', 'ilike', '" + searchKey+"')";
            }
            //Fields = new List<string> { "id", "name", "list_price", "image_128", "public_categ_ids" };
            Domain = "["+filter+"]";// "[('is_published', '=', True)" + (filter < 0 ? "" : ", ('public_categ_ids', 'in', [" + filter + "])") + "]";
            Limit = pageSize;
            Offset = startIndex;
        }
    }

    public class OdooAddWishListRequestDto
    {
        public OdooAddWishListRequestDto(int productID)
        {
            ProductID = productID;
        }
        [JsonProperty("product_id")]
        public int ProductID;
    } 
    public class OdooRemoveWishListRequestDto
    {
        public OdooRemoveWishListRequestDto(int wishID)
        {
            WishID = wishID;
        }
        [JsonProperty("wish_id")]
        public int WishID;
    }
}
