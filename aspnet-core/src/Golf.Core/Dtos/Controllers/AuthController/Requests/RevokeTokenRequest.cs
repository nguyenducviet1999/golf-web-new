using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AuthController.Requests
{
    public class RevokeTokenRequest
    {
        public Guid GolferID { get; set; }
        public string Token { get; set; }
    }
}
