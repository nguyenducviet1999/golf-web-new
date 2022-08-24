using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Common.Odoo.OdooResponse
{
    public class OdooResponse<T>
    {
        [JsonProperty("jsonrpc")]
        public string JsonRpc { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("result")]
        public T Result { get; set; }
        [JsonProperty("error")]
        public DataResponseBase Error { get; set; }
    }
    public class OdooResult<T>
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public T Data { get; set; }
    }
    public class DataResponseBase
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class OdooObject
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

}
