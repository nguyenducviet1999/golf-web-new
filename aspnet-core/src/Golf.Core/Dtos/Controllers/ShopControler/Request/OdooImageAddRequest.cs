using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Request
{

    public class OdooImageAddRequest
    {
        public string ResID { get; set; }
        public IFormFile ImageFile { get; set; }
        public MultipartFormDataContent FormData()
        {
            var ms = new MemoryStream();
            ImageFile.CopyTo(ms);
            var tmp = new MultipartFormDataContent();
            tmp.Add(new StringContent("product.template"), "res_model");
            tmp.Add(new StringContent(ImageFile.FileName), "name");
            tmp.Add(new StringContent(ResID), "res_id");
            //tmp.Add(new file()// (ResID), "res_id");
            tmp.Add(new ByteArrayContent(ms.ToArray()), "file", ImageFile.FileName);
            return tmp;
        }
    }
    public class OdooImageRemoveRequestDto
    {
        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; }
        [JsonProperty("method")]
        public string Method { get; set; }
        [JsonProperty("params")]
        public Params Params { get; set; }
        public OdooImageRemoveRequestDto(string imageID, string accessToken)
        {
            Jsonrpc = "2.0";
            Method = "call";
            Params = new Params()
            {
                AttachmentID = imageID,
                AccessToken = accessToken
            };
        }
    }

    public class Params
    {
        [JsonProperty("attachment_id")]
        public string AttachmentID { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

    }
    public class OdooImageRemoveRequest
    {
        public string ImageID { get; set; }
        public string AccessToken { get; set; }
        public OdooImageRemoveRequestDto ToOdooImageRemoveRequestDto()
        {
            return new OdooImageRemoveRequestDto(ImageID, AccessToken);
        }

    }
}
