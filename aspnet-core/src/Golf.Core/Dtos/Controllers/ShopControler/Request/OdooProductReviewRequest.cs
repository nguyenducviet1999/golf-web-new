using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{
    public class OdooProductReviewRequestDto
    {
        [JsonProperty("res_id")]
        public int ProductID;
        [JsonProperty("res_model")]
        public string ResModel;
        [JsonProperty("message")]
        public string Content;
        [JsonProperty("rating")]
        public int RatingPoint;
        [JsonProperty("attachment_ids")]
        public List<string> ImageIDs;
    }
    public class OdooProductReviewRequest
    {
        public string Content { get; set; }
        public int RatingPoint { get; set; }
        public List<IFormFile> Images { get; set; }
        public OdooProductReviewRequestDto ToOdooProductReviewRequestDto(int prodcutID, List<string> imageIDs)
        {
            OdooProductReviewRequestDto result = new OdooProductReviewRequestDto();
            result.ProductID = prodcutID;
            result.ResModel = "product.template";
            result.Content = Content;
            result.RatingPoint = RatingPoint;
            result.ImageIDs = imageIDs;
            return result;
            
        }
    }
   
}
