using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using Golf.Domain.GolferData;
using Golf.Domain.Shared.Setting;
using Golf.Domain.Shared.AccessToken;
using Golf.Services.OdooAPI;
using System.Net;

namespace Golf.HttpApi.Host.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly AccessTokenHandler _accessTokenHandler;
        private readonly OdooAPIService _odooAPIService;

        public JwtMiddleware(
            OdooAPIService odooAPIService,
            AccessTokenHandler accessTokenHandler,
            RequestDelegate next,
            IOptions<AppSettings> appSettings,
            ILoggerFactory loggerFactory)
        {
            _odooAPIService = odooAPIService;
            _accessTokenHandler = accessTokenHandler;
            _next = next;
            _appSettings = appSettings.Value;
            _logger = loggerFactory.CreateLogger<JwtMiddleware>();
        }

        public async Task Invoke(HttpContext context, UserManager<Golfer> golferManager)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //set odoo cookie
            _odooAPIService.Cookies.Add(new Uri(_appSettings.BaseOdooUrl), new Cookie(_appSettings.OdooCookieKey, context.Request.Cookies[_appSettings.OdooCookieKey]));//   .Add(new Cookie("session_id", context.Request.Cookies["session_id"]));
            _odooAPIService.Cookies.Add(new Uri(_appSettings.BaseOdooUrl), new Cookie(_appSettings.OdooUserID, context.Request.Cookies[_appSettings.OdooUserID]));
            _odooAPIService.Cookies.Add(new Uri(_appSettings.BaseOdooUrl), new Cookie(_appSettings.OdooPartnerID, context.Request.Cookies[_appSettings.OdooPartnerID]));
            //_shopService._odooAPIService.Cookies.Add(new Cookie("session_id", Request.Cookies["MyTestCookie"]));
            if (token != null)
                await attachGolferToContext(context, token, golferManager);

            await _next(context);
        }

        private async Task attachGolferToContext(HttpContext context, string token, UserManager<Golfer> golferManager)
        {
            try
            {
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                //tokenHandler.ValidateToken(token, new TokenValidationParameters
                //{
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = new SymmetricSecurityKey(key),
                //    ValidateIssuer = false,
                //    ValidateAudience = false,
                //    ClockSkew = TimeSpan.Zero,
                //    RequireExpirationTime = true
                //}, out SecurityToken validatedToken);

                //var jwtToken = (JwtSecurityToken)validatedToken;
                //var golferId = jwtToken.Claims.First(x => x.Type == "id").Value;
                var golferId = _accessTokenHandler.Handle(token);
                var golfer = await golferManager.FindByIdAsync(golferId);
                //context.User = golfer;
                context.Items["Golfer"] = golfer;
                context.Items["Roles"] = await golferManager.GetRolesAsync(golfer);
            }
            catch (Exception e)
            {
                _logger.LogError($"{e}");
            }
        }
    }
}
