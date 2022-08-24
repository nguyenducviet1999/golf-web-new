using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Options;
using AutoMapper;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Common;
using Golf.Services;
using Golf.Core.Common.Golfer;

using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.AuthController.Requests;
using Golf.Core.Dtos.Controllers.AuthController.Responses;
using Golf.Domain.Shared.Setting;
using Microsoft.AspNetCore.Http;
using Golf.Core.Dtos.Controllers.ProfileController.Requests;
using System.Net;
using System.Security.Cryptography;
using Golf.HttpApi.Host.Helpers;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly AuthService _authService;
        private readonly SeedDataService _seedDataService;
        private readonly AppSettings _appSettings;
        private readonly ProfileService _profileService;

        public AuthController(ProfileService profileService,
            IOptions<AppSettings> appSettings,
            SeedDataService seedDataService,
            AuthService authService,
            IMapper mapper
        )
        {
            _profileService = profileService;
            _seedDataService = seedDataService;
            _appSettings = appSettings.Value;
            _authService = authService;
            _mapper = mapper;
        }

        /// <summary>
        /// API đăng kí tài khoản
        /// </summary>
        /// <param name="request">dữ liệu đầu vào để đăng kí</param>
        /// <returns>AccessToken người dùng</returns>
        // POST api/Auth/Register
        //[HttpPost("Register")]
        //public async Task<ActionResult<DefaultAuthResponse>> Register(RegisterRequest request)
        //{
        //    if (request.Validate() == false)
        //    {
        //        throw new BadRequestException("Bad Request");
        //    }
        //    var golfer = await _authService.Register(request);
        //    return Ok(new DefaultAuthResponse
        //    {
        //        Golfer = _mapper.Map<MinimizedGolfer>(golfer),
        //        AccessToken = generateJwtToken(golfer, Token.AccessToken),
        //        RefreshToken = generateJwtToken(golfer, Token.RefreshToken)
        //    });
        //}

        // POST api/Auth/Login
        /// <summary>
        /// APi đăng nhập tài khoản
        /// </summary>
        /// <param name="request"></param>
        /// <returns>AccessToken người dùng</returns>
          [HttpPost("loginn")]
        public async Task<ActionResult<DefaultAuthResponse>> Login(LoginRequest request)
        {
            if (request.Validate() == false)
            {
                throw new BadRequestException("Bad Request");
            }
            var golfer = await _authService.LoginDefault(request);


            var tokenRresponse = _authService.GetNewTokenResponse(golfer, ipAddress());
            return Ok(new DefaultAuthResponse
            {

                Golfer = _mapper.Map<MinimizedGolfer>(golfer),
                AccessToken = tokenRresponse.AccessToken,
                RefreshToken = tokenRresponse.RefreshToken
            });
        }
        [HttpPost("AdminLogin")]
        public async Task<ActionResult<AdminAuthResponse>> AdminLogin(LoginRequest request)
        {
            if (request.Validate() == false)
            {
                throw new BadRequestException("Bad Request");
            }
            var golfer = await _authService.LoginAdmin(request);
            // var role= await golferManager.GetRolesAsync(golfer)
            var tokenRresponse = _authService.GetNewTokenResponse(golfer, ipAddress());
            return Ok(new DefaultAuthResponse
            {

                Golfer = _mapper.Map<MinimizedGolfer>(golfer),
                AccessToken = tokenRresponse.AccessToken,
                RefreshToken = tokenRresponse.RefreshToken
            });
        }

        // POST api/Auth/GoogleAuthentication
        [HttpPost("GoogleAuthentication")]
        public async Task<ActionResult<DefaultAuthResponse>> GoogleAuthentication(GoogleAuthenticationRequest request)
        {
            var golfer = await _authService.GoogleAuthentication(request, _appSettings.SupportedClientIds);
            var tokenRresponse = _authService.GetNewTokenResponse(golfer, ipAddress());
            return Ok(new DefaultAuthResponse
            {

                Golfer = _mapper.Map<MinimizedGolfer>(golfer),
                AccessToken = tokenRresponse.AccessToken,
                RefreshToken = tokenRresponse.RefreshToken
            });
        }

        // // POST api/Auth/SeedGolfer
        // [HttpPost("SeedGolfer")]
        // public async Task<ActionResult<DefaultAuthResponse>> SeedGolfer()
        // {
        //     var golfer = await _seedDataService.SeedGolfer();
        //     return Ok(new DefaultAuthResponse
        //     {
        //         Golfer = _mapper.Map<MinimizedGolfer>(golfer),
        //         AccessToken = generateJwtToken(golfer, Token.AccessToken),
        //         RefreshToken = generateJwtToken(golfer, Token.RefreshToken)
        //     });
        // }
        [HttpPost("Login")]
        public async Task<ActionResult<DefaultAuthResponse>> Login(LoginOdooRequest request)
        {
            if (request.Validate() == false)
            {
                throw new BadRequestException("Bad Request");
            }
            var url = _appSettings.BaseOdooUrl + _appSettings.AuthLogin;
            var golfer = await _authService.LoginOdoo(request, url, _appSettings.DbOdoo);
            foreach (Cookie cookie in _authService._cookieContainer.GetCookies(new Uri(_appSettings.BaseOdooUrl)))
            {
                setCookie(cookie.Name,cookie.Value);
            }
            // setTokenCookie(_authService._cookieContainer.GetCookies(new Uri(_appSettings.BaseOdooUrl)).Find);
            var tokenRresponse = _authService.GetNewTokenResponse(golfer, ipAddress());
            return Ok(new DefaultAuthResponse
            {

                Golfer = _mapper.Map<MinimizedGolfer>(golfer),
                AccessToken = tokenRresponse.AccessToken,
                RefreshToken = tokenRresponse.RefreshToken
            });
        }
        [HttpPost("SignUp")]
        public async Task<ActionResult<DefaultAuthResponse>> SignUp(SignUpRequest request)
        {
            if (request.Validate() == false)
            {
                throw new BadRequestException("Bad Request");
            }
            var url = _appSettings.BaseOdooUrl + _appSettings.AuthSignUp;
            
            var golfer = await _authService.SignupOdoo(request, url, _appSettings.DbOdoo);
            foreach (Cookie cookie in _authService._cookieContainer.GetCookies(new Uri(_appSettings.BaseOdooUrl)))
            {
                setCookie(cookie.Name, cookie.Value);
            }
            var tokenRresponse = _authService.GetNewTokenResponse(golfer, ipAddress());
            return Ok(new DefaultAuthResponse
            {

                Golfer = _mapper.Map<MinimizedGolfer>(golfer),
                AccessToken = tokenRresponse.AccessToken,
                RefreshToken = tokenRresponse.RefreshToken
            });
        }
        [HttpPost("ForgotPassword/{login}")]
        public async Task<ActionResult<bool>> ForgotPassword(string login)
        {
            await _authService.ForgotPassword(login);
            return Ok(true);
        }
        [HttpPost("Password/NewPassword")]
        public async Task<ActionResult<bool>> NewPassword(NewPasswordRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            await _authService.NewPassword(request);
            return Ok(true);
        }
        [HttpPost("RefreshToken")]
        public ActionResult<TokenResponse> RefreshToken(RevokeTokenRequest request)
        {
            var refreshToken = request.Token;
            var response = _authService.RefreshToken( request.GolferID,refreshToken, ipAddress());
            return Ok(response);
        }
        [Authorize]
        [HttpPost("RevokeToken")]
        public ActionResult RevokeToken(RevokeTokenRequest request)
        {
            // accept refresh token in request body or cookie
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var token = request.Token;

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            _authService.RevokeToken(currentGolfer.Id, token, ipAddress());
            return Ok(new { message = "Token revoked" });
        }
        private void setCookie(string key,string value)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(15)
            };
            Response.Cookies.Append(key, value, cookieOptions);
        }
        private string ipAddress()
        {
            // get source ip address for the current request
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        //[ApiExplorerSettings(IgnoreApi = true)]
        //private string generateJwtToken(Golfer golfer, Token tokenType)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] { new Claim("id", golfer.Id.ToString()), new Claim("golfername", golfer.UserName) }),
        //        Expires = DateTime.UtcNow.AddDays(tokenType == Token.AccessToken ? _appSettings.Expires : _appSettings.RefreshExpires),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

    }
}
