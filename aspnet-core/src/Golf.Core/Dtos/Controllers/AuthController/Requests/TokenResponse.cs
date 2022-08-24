using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AuthController.Requests
{
    public class TokenResponse
    {
        public TokenResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
