using Golf.Domain.Shared.Golfer.UserSetting;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Golf.Domain.GolferData
{
    public class Golfer : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public double StartHandicap { get; set; }
        public double Handicap { get; set; }
        public double IDX { get; set; }
        public int OdooUserID { get; set; }
        public int OdooPartnerID { get; set; }

        [Column(TypeName = "jsonb")]
        public List<Guid> CourseFavorites { get; set; } = new List<Guid>();
        [Column(TypeName = "jsonb")]
        public UserSetting Setting { get; set; } = new UserSetting();
        [Column(TypeName = "jsonb")]
        public List<RefreshToken> RefreshTokens { get; set; }
        public string GetFullName()
        {
            return FirstName + " " + LastName;
        }
    }
}