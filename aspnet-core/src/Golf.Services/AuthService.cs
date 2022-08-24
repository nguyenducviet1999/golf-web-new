using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;

using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Golfer;

using Golf.Core.Common;
using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.AuthController.Requests;
using Golf.Domain.GolferData;
using System.Text;
using Golf.Core.Dtos.Controllers.AuthController.Responses;
using Golf.Services.OdooAPI;
using Golf.Domain.Shared.OdooAPI;
using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Core.Dtos.Controllers.AdminController.Account.Request;
using Golf.Core.Dtos.Controllers.ProfileController.Requests;
using System.Security.Cryptography;
using Golf.Domain.Shared.Setting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Golf.Domain.Shared.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Golf.Services
{
    public class AuthService
    {
        private readonly UserManager<Golfer> _golferManager;
        private readonly GolferRepository _golferRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ProfileRepository _profileRepository;
        private readonly SignInManager<Golfer> _signInManager;
        private const string GoogleApiTokenInfoUrl = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly OdooAPIService _odooAPIService;
        public CookieContainer _cookieContainer;
        private readonly AppSettings _appSettings;

        public AuthService(
            GolferRepository golferRepository,
            OdooAPIService  odooAPIService,
            IOptions<AppSettings> appSettings,
            UserManager<Golfer> golferManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ProfileRepository profileRepository,
            SignInManager<Golfer> signInManager,
            DatabaseTransaction databaseTransaction)
        {
            _appSettings = appSettings.Value;
            _golferRepository = golferRepository;
            _odooAPIService = odooAPIService;
            _golferManager = golferManager;
            _roleManager = roleManager;
            _profileRepository = profileRepository;
            _signInManager = signInManager;
            _databaseTransaction = databaseTransaction;
        }

        async public Task<Golfer> Register(RegisterRequest request)
        {
            _databaseTransaction.BeginTransaction();
            var golfer = await _golferManager.FindByNameAsync(request.PhoneNumber);
            if (golfer != null)
            {
                throw new UnauthorizedAccessException("Account Exists");
            }
            try
            {
                var newGolfer = new Golfer()
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.PhoneNumber,
                    NormalizedUserName = request.PhoneNumber,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Avatar = "",
                    Cover = "",
                    Handicap = request.Handicap,
                    StartHandicap = request.Handicap,
                    IDX = 0.0
                };
                var result = await _golferManager.CreateAsync(newGolfer, request.Password);
                if (result.Succeeded)
                {
                    if (request.Roles == null || request.Roles.Count == 0)
                    {
                        request.Roles = new List<string>() { RoleNormalizedName.Golfer };
                    }
                    await _golferManager.AddToRolesAsync(newGolfer, request.Roles);
                    _profileRepository.Add(new Profile
                    {
                        ID = newGolfer.Id,
                        CountryID = request.CountryID,
                        ShirtSize = -1,
                        ShoesSize = -1,
                        PantsSize = -1

                    });
                    await _databaseTransaction.Commit();
                    return newGolfer;
                }
                else
                {
                    throw new Exception(result.Errors.ToString());
                }
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception("Create Account Error: " + exception);
            }
        }

        /// <summary>
        /// Đăng nhập với tài khoản và mật khẩu
        /// </summary>
        /// <param name="request">Dữ liệu tên tài khoản và mật khẩu</param>
        /// <returns>dữ liệu người dùng</returns>
        async public Task<Golfer> LoginAdmin(LoginRequest request)
        {
            var golfer = await _golferManager.FindByNameAsync(request.UserName);
            if (golfer == null)
            {
                throw new UnauthorizedAccessException("Account Doesn't Exist");
            }
            var result = await _signInManager.PasswordSignInAsync(golfer.UserName, request.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var role = await _golferManager.GetRolesAsync(golfer);
                var tmp = false;
                foreach (var i in role)
                {
                    if (i == RoleName.CourseAdmin || i == RoleName.SystemAdmin)
                    {
                        tmp = true;
                    }
                }
                if (tmp == true)
                    return golfer;
                else
                {
                    throw new UnauthorizedAccessException("Unauthorized");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Unauthorized");
            }
        }
        async public Task<Golfer> LoginDefault(LoginRequest request)
        {
            var golfer = await _golferManager.FindByNameAsync(request.UserName);
            if (golfer == null)
            {
                throw new UnauthorizedAccessException("Account Doesn't Exist");
            }
            var result = await _signInManager.PasswordSignInAsync(golfer.UserName, request.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return golfer;
            }
            else
            {
                throw new UnauthorizedAccessException("Wrong Password");
            }
        }
        /// <summary>
        ///  Đăng nhập với google
        /// </summary>
        /// <param name="request"></param>
        /// <param name="SupportedClientIds"></param>
        /// <returns></returns>
        public async Task<Golfer> GoogleAuthentication(GoogleAuthenticationRequest request, List<string> SupportedClientIds)
        {
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, request.AccessToken));
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception e)
            {
                throw new Exception($"{e.Message}");
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                throw new BadRequestException("Access Token Invalid");
            }
            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var googleApiTokenInfo = JsonConvert.DeserializeObject<GoogleApiTokenInfo>(response);
            if (!SupportedClientIds.Contains(googleApiTokenInfo.aud))
            {
                throw new ForbiddenException("Client is not supported");
            }
            var golfer = await _golferManager.FindByEmailAsync(googleApiTokenInfo.email);
            if (golfer != null)
            {
                return golfer;
            }
            else
            {
                var newGolfer = new Golfer()
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = "",
                    UserName = googleApiTokenInfo.email,
                    NormalizedUserName = googleApiTokenInfo.email,
                    FirstName = googleApiTokenInfo.given_name,
                    LastName = googleApiTokenInfo.family_name,
                    Email = googleApiTokenInfo.email,
                    Avatar = "",
                    Cover = "",
                    Handicap = 0.0,
                    IDX = 0.0
                };
                var result = await _golferManager.CreateAsync(newGolfer, googleApiTokenInfo.email);
                if (result.Succeeded)
                {
                    await _golferManager.AddToRolesAsync(newGolfer, new List<string>() { RoleNormalizedName.Golfer });
                    _profileRepository.Add(new Profile
                    {
                        ID = newGolfer.Id,
                    });
                    return newGolfer;
                }
                else
                {
                    throw new Exception("Create Account Error: " + result);
                }
            }
        }

        public async Task<Golfer> LoginOdoo(LoginOdooRequest request, string url, string db)
        {

            var loginOdooDto = new LoginOdooDto()
            {
                Login = request.Email.Trim().ToLower(),
                Password = request.Password,
                Db = db,
                IsMemberGolf = true
            };
            //var json = JsonConvert.SerializeObject(loginOdooDto);
            //var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
            //var client = new HttpClient();
            //var response = await client.PostAsync(url, stringContent);
            //var content = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<OdooResponse<OdooResult<LoginOdooResponse>>>(content);
            var result =await _odooAPIService.CallAPI<LoginOdooDto, OdooResponse<OdooResult<LoginOdooResponse>>>(APIMethod.POST,url, loginOdooDto);      
            //add cookie
            _odooAPIService.Cookies.Add(new Uri(_odooAPIService._appSettings.BaseOdooUrl), new Cookie(_odooAPIService._appSettings.OdooUserID, result.Result.Data.User.Id.ToString()));
            _odooAPIService.Cookies.Add(new Uri(_odooAPIService._appSettings.BaseOdooUrl), new Cookie(_odooAPIService._appSettings.OdooPartnerID, result.Result.Data.User.PartnerID.ToString()));
            _cookieContainer = _odooAPIService.Cookies;
            //golf login
            var golfer = await _golferManager.FindByNameAsync(result.Result.Data.User.Email);
            if (golfer == null)
            {
                throw new UnauthorizedAccessException("Golf account isn't exit");
            }   
            return golfer;
        }
        public async Task<Golfer> SignupOdoo(SignUpRequest request, string url, string db)
        {
            var user = await _golferManager.FindByNameAsync(request.Email);
            if (user != null)
            {
                throw new BadRequestException("Golf account already exists");
            }
            var signUpOdooDto = new SignUpOdooDto()
            {
                Login = request.Email.ToLower(),
                Password = request.Password,
                Name = request.GetFulName(),
                Phone = request.PhoneNumber,
                Db = db,
                CountryID=request.CountryID,
                StateID=request.CountryID,
                City = request.City,
                Street = request.Street,
                Street2 = request.Street,
                Zip = request.Zip,
                CustomerTypes = new List<int>() { 1},
            };
            var result =await _odooAPIService.CallAPI<SignUpOdooDto,  OdooResponse<OdooResult<SignUpOdooResponse>>>(APIMethod.POST, url, signUpOdooDto);
            //signup
            //add cookie
            _odooAPIService.Cookies.Add(new Uri(_odooAPIService._appSettings.BaseOdooUrl), new Cookie(_odooAPIService._appSettings.OdooUserID, result.Result.Data.User.Id.ToString()));
            _odooAPIService.Cookies.Add(new Uri(_odooAPIService._appSettings.BaseOdooUrl), new Cookie(_odooAPIService._appSettings.OdooPartnerID, result.Result.Data.User.PartnerID.ToString()));
            _cookieContainer = _odooAPIService.Cookies;
            //golf signup
            _databaseTransaction.BeginTransaction();
            try
            {
                var firstName = request.FirstName;
                var lastName = request.LastName;
                var newGolfer = new Golfer()
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = request.PhoneNumber,
                    UserName = request.Email,
                    Email = request.Email,
                    NormalizedUserName = request.PhoneNumber,
                    FirstName = firstName,
                    LastName = lastName,
                    Avatar = "",
                    Cover = "",
                    Handicap = request.Handicap,
                    StartHandicap = request.Handicap,
                    IDX = 0.0,
                    OdooPartnerID = result.Result.Data.User.PartnerID,
                    OdooUserID = result.Result.Data.User.Id,
                };
                var tmp = await _golferManager.CreateAsync(newGolfer, request.Password);
                if (tmp.Succeeded)
                {
                    
                    await _golferManager.AddToRolesAsync(newGolfer, new List<string>() { RoleNormalizedName.Golfer });
                    _profileRepository.Add(new Profile
                    {
                        ID = newGolfer.Id,
                        CountryID = request.CountryID,
                        StateID=request.StateID,
                        ShirtSize = -1,
                        ShoesSize = -1,
                        PantsSize = -1

                    });
                    await _databaseTransaction.Commit();
                    return newGolfer;
                }
                else
                {
                    throw new Exception(tmp.Errors.ToString());
                }
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception("Create Account Error: " + exception);
            }
            //var result = await _signInManager.PasswordSignInAsync(golfer.UserName, request.Password, false, lockoutOnFailure: false);
            //if (result.Succeeded)
            //{
            //    return golfer;
            //}
            //else
            //{
            //    throw new UnauthorizedAccessException("Wrong Password");
            //}
        }

        public async Task<bool> NewPassword(NewPasswordRequest request)
        {
            //odoo update
            var newPasswordDto = request.GetNewPasswordDto();
            var respone = await _odooAPIService.CallAPI<NewPasswordDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getChangePasswordUrl(), newPasswordDto);
            if (respone.Result.Code != 200)
            {
                throw new Exception(respone.Result.Data.Message);
            }
            //odoo update
            return true;
        }
        public async Task<bool> ForgotPassword(string login)
        {
            //odoo update
            ForgotPasswordDto forgotPasswordDto = new ForgotPasswordDto(login);
            var respone = await _odooAPIService.CallAPI<ForgotPasswordDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getForgotPasswordUrl(), forgotPasswordDto);
            if (respone.Result.Code != 200)
            {
                throw new Exception(respone.Result.Data.Message);
            }
            //odoo update
            return true;
        }
        //hep

        [ApiExplorerSettings(IgnoreApi = true)]
        private string generateJwtToken(Golfer golfer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", golfer.Id.ToString()), new Claim("golfername", golfer.UserName) }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            // generate token that is valid for 7 days
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddDays(_appSettings.RefreshExpires),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;
        }
        public TokenResponse GetNewTokenResponse(Golfer golfer,string ipAddress)
        {
            var jwtToken = generateJwtToken(golfer);
            var refreshToken = GenerateRefreshToken(ipAddress);
            if (golfer.RefreshTokens == null) golfer.RefreshTokens = new List<RefreshToken>();
            golfer.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(golfer);

            // save changes to db
            try
            {
                _golferRepository.Update(golfer);
            }
            catch(Exception e)
            {

            }
            return new TokenResponse(jwtToken,refreshToken.Token);
        }
        public TokenResponse RefreshToken(Guid currentGolferID,string token, string ipAddress)
        {
            var user = _golferRepository.Get(currentGolferID);// getUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                revokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _golferRepository.Update(user);
            }

            if (!refreshToken.IsActive)
                throw new Exception("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = rotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            removeOldRefreshTokens(user);

            // save changes to db
            _golferRepository.Update(user);
            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new TokenResponse(jwtToken, newRefreshToken.Token);
        }
        /// <summary>
        /// vô hiệu hóa refreshtoken
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ipAddress"></param>
        public void RevokeToken(Guid currentGolferID,string token, string ipAddress)
        {
            var user = _golferRepository.Get(currentGolferID);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new Exception("Invalid token");

            // revoke token and save
            revokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _golferRepository.Update(user);
        }
       
        /// <summary>
        /// Tạo mã refresh mới từ mã cũ và ip mới
        /// </summary>
        /// <param name="refreshToken">mã cũ</param>
        /// <param name="ipAddress">địa chỉ ip</param>
        /// <returns></returns>
        private RefreshToken rotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            revokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }
        /// <summary>
        /// xóa các mã ko còn hoạt động và tạo được hơn 2 ngày
        /// </summary>
        /// <param name="golfer"></param>
        private void removeOldRefreshTokens(Golfer golfer)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            golfer.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }
        /// <summary>
        /// hàm đệ quy xóa vô hiệu hóa các refreshtoken thay thế refresh đã hết hạn mà bị dùng lại
        /// </summary>
        /// <param name="refreshToken">Mã hết hạn</param>
        /// <param name="golfer"></param>
        /// <param name="ipAddress"></param>
        /// <param name="reason"></param>
        private void revokeDescendantRefreshTokens(RefreshToken refreshToken, Golfer golfer, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = golfer.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken.IsActive)
                    revokeRefreshToken(childToken, ipAddress, reason);
                else
                    revokeDescendantRefreshTokens(childToken, golfer, ipAddress, reason);//vô hiệu hóa các refreshtoken thay thế 
            }
        }
        /// <summary>
        /// vô hiệu hóa môt refreshtoken
        /// </summary>
        /// <param name="token">token muốn vô hiệu</param>
        /// <param name="ipAddress">ip vô hiệu</param>
        /// <param name="reason">Nguyên nhân</param>
        /// <param name="replacedByToken">Thay thế bởi token</param>
        private void revokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
