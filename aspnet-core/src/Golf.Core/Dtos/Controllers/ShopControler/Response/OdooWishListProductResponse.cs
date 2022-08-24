using Golf.Domain.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooWishListProductResponseDto
    {
        [JsonProperty("id")]
        public int WishListID;
        [JsonProperty("product_id")]
        public string StringID;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("default_code")]
        public string Code;
        [JsonProperty("description_sale")]
        public string DescriptionSale;
        [JsonProperty("type")]
        public string Type;
        //[JsonProperty("public_categ_ids")]
        //public string Category;
        [JsonProperty("image_128")]
        public string Image;
        [JsonProperty("list_price")]
        public double Price;
        public int ProductID => int.Parse(StringHelper.OdooGetIntByString(StringID));
    }
    public class OdooWishListProductResponse
    {
        public int WishListID;
        public int ProductID;
        public string Name;
        public string Code;
        public string Type;
        public string Image;
        public double Price;
    }
}
