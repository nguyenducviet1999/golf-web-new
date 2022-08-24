using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooPromotionCodeRequest
    {
        [JsonProperty("promo_code")]
        public string Code;
        public OdooPromotionCodeRequest(string promotionCode)
        {
            Code = promotionCode;
        }
    }
}
