using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooAddToCartRequestDto
    {
        [JsonProperty("product_id")]
        public int ID { get; set; }
        [JsonProperty("qty")]
        public int Quantity { get; set; }
        public OdooAddToCartRequestDto(int id, int quantity)
        {
            ID = id;
            Quantity = quantity;
        }
    }
    public class OdooAddToCartRequest
    {
        public int ID { get; set; }
        public int Quantity { get; set; }

        public OdooAddToCartRequestDto ToOdooCartRequestDto()
        {
            return new OdooAddToCartRequestDto(ID, Quantity);
        }
    }

    public class OdooRemoveFromCartRequestDto
    {
        [JsonProperty("product_id")]
        public List<int> ProductIDs { get; set; }
        public OdooRemoveFromCartRequestDto(List<int> productIDs)
        {
            this.ProductIDs = productIDs;
        }
    }
    public class OdooRemoveFromCartRequest
    {
        public List<int> ProductIDs { get; set; }
        public OdooRemoveFromCartRequestDto ToRemoveFromCartRequestDto()
        {
            return new OdooRemoveFromCartRequestDto(this.ProductIDs);
        }
    }
}
