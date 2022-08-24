using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
   public class OdooGetCategorysRequestĐto
    {
        [JsonProperty("fields")]
        public List<string> Fields;
        public OdooGetCategorysRequestĐto()
        {
            Fields = new List<string>() { "id", "name", "sequence" };
        }
    }
}
