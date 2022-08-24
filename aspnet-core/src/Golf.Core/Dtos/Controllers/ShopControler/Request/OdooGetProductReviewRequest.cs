using Golf.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooGetProductReviewRequestDto
    {
      public  OdooGetProductReviewRequestDto(int resID,int startIndex)
        {
            Paramms tmp = new Paramms();
            tmp.ResModel = "product.template";
            tmp.ResID = resID;
            tmp.Limit = Const.PageSize;
            tmp.Offset = startIndex;
            tmp.AllowComposer = 1;
            tmp.Domain =new List<string>();
            tmp.RatingInclude = true;
            this.Params = tmp;
        }
        [JsonProperty("params")]
        Paramms Params;

    }
    public class Paramms
    {
        [JsonProperty("res_model")]
        public string ResModel;
        [JsonProperty("res_id")]
        public int ResID;
        [JsonProperty("limit")]
        public int Limit;
        [JsonProperty("offset")]
        public int Offset;
        [JsonProperty("allow_composer")]
        public int AllowComposer;
        [JsonProperty("domain")]
        public List<string> Domain;
        [JsonProperty("rating_include")]
        public bool RatingInclude;
    }

  
}
