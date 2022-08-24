using Golf.Core.Common.Odoo.OdooResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ShopControler.Response
{
    public class OdooAdressResponseDto
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("street")]
        public string Street { get; set; }
        [JsonProperty("street2")]
        public string Street2 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country_id")]
        public List<string> Country { get; set; }
        [JsonProperty("state_id")]
        public List<string> State { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("is_default_shipping")]
        public bool IsDefaultShipping { get; set; }
    }
    public class OdooAdressResponse
    {
        public int ID { get; set; }//định danh địa chỉ
        public string Name { get; set; }//tên địa chỉ
        public string Phone { get; set; }//số điện thoại người nhận
        public string Email { get; set; }//Địa chỉ thư điện tử
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }//thành phố
        public OdooObject Country { get; set; } = new OdooObject();//định danh quốc gia
        public OdooObject State { get; set; } = new OdooObject();//
        public string Type { get; set; }//loại
        public string Zip { get; set; }//mà bưu điện
        public bool IsDefaultShipping { get; set; }//có phải vị trí mặc định nhận hàng
    }
   
}
