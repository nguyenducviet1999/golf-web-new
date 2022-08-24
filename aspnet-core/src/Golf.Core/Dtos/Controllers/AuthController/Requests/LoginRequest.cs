using System;
using System.Collections.Generic;
using System.Text;
using Golf.Core.Dtos.Request;
using Newtonsoft.Json;

namespace Golf.Core.Dtos.Controllers.AuthController.Requests
{
    public class LoginRequest : IRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public override bool Validate()
        {
            return true;
        }
    }

    public class LoginOdooRequest : IRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public override bool Validate()
        {
            return true;
        }
    } 
    //public class SignUpOdooRequest : IRequest
    //{
    //    "login": "zzthebaozz@gmail.com",
    //"password": "123",
    //"db": "odoo",
    //"name": "Phong Dương",
    //"phone": "0123456789",
    //"street": "Địa chỉ 1",
    //"street2": "Địa chỉ 2",
    //"zip": "123",
    //"city": "Hà Nội"
    //    public string Email { get; set; }
    //    public string Password { get; set; }

    //    public override bool Validate()
    //    {
    //        return true;
    //    }
    //}

    public class LoginOdooDto
    {
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("db")]
        public string Db { get; set; }
        [JsonProperty("is_member_golf")]
        public bool IsMemberGolf { get; set; }
    }
}
