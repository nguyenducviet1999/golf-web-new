using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooImageResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("mimetype")]
        public string MimeType { get; set; }
        [JsonProperty("file_size")]
        public int FileSize { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
    }
    public class OdooImageResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public int FileSize { get; set; }
        public string AccessToken { get; set; }
        
    }
}
