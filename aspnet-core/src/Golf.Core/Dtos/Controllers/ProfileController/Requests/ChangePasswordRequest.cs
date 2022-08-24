using Microsoft.AspNetCore.Http;
using Golf.Core.Dtos.Request;
using Newtonsoft.Json;

namespace Golf.Core.Dtos.Controllers.ProfileController.Requests
{
    public class ChangePasswordRequest : IRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public override bool Validate()
        {
            return false;
        }
        public ChangePasswordDto GetChangePasswordDto()
        {
            ChangePasswordDto changePasswordDto = new ChangePasswordDto();
            changePasswordDto.OldPassword = CurrentPassword;
            changePasswordDto.NewPassword = NewPassword;
            return changePasswordDto;
        }
    } 
    public class ChangePasswordDto 
    {
       
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("old_password")]
        public string OldPassword { get; set; }
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
       
    } 
    public class ForgotPasswordDto
    {
       
        [JsonProperty("login")]
        public string Login { get; set; }
        public ForgotPasswordDto(string login)
        {
            Login = login;
        }
       
    } 
    public class NewPasswordRequest : IRequest
    {
        public string Login { get; set; }
        public int Otp { get; set; }
        public string NewPassword { get; set; }
        public override bool Validate()
        {
            return false;
        }
        public NewPasswordDto GetNewPasswordDto()
        {
            NewPasswordDto changePasswordDto = new NewPasswordDto();
            changePasswordDto.Otp =Otp ;
            changePasswordDto.NewPassword = NewPassword;
            changePasswordDto.Login = Login;
            return changePasswordDto;
        }
    } 
    public class NewPasswordDto 
    {
       
        [JsonProperty("login")]
        public string Login { get; set; }
        [JsonProperty("otp")]
        public int Otp { get; set; }
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
       
    }
}
