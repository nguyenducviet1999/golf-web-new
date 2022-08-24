using Golf.Domain.Shared.Golfer;
using Golf.Domain.Courses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using Golf.Domain.GolferData;
using System.Threading.Tasks;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.System;

namespace Golf.EntityFrameworkCore
{
    class GolfSeedData
    {
        private readonly ModelBuilder _modelBuilder;

        public GolfSeedData(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void SeedRoles()
        {
            this._modelBuilder.Entity<IdentityRole<Guid>>()
                .HasData(
                    new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "System Admin",
                        NormalizedName = RoleNormalizedName.SystemAdmin,
                        ConcurrencyStamp = ""
                    }, new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "Course Admin",
                        NormalizedName = RoleNormalizedName.CourseAdmin,
                        ConcurrencyStamp = ""
                    }, new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "GSA",
                        NormalizedName = RoleNormalizedName.GSA,
                        ConcurrencyStamp = ""
                    }, new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = "Golfer",
                        NormalizedName = RoleNormalizedName.Golfer,
                        ConcurrencyStamp = ""
                    });
        }

        public void SeedCourseExtension()
        {
            var courseExtensions = new List<CourseExtension>();
            string[] courseExtensionNames = {
                "Hire caddy",
                "Rent golf cart",
                "Driving range",
                "Pro shop",
                "Rent clubs",
                "Rent shoes",
                "Rent umbrella",
                "Locker",
                "Caddy",
                "Golf cart",
                "Golf cart surcharge",
                "Club house",
                "Restaurant",
                "Palace"
            };

            for (int i = 0; i < courseExtensionNames.Length; i++)
            {
                courseExtensions.Add(new CourseExtension
                {
                    ID = i + 1,
                    Name = courseExtensionNames[i],
                    Icon = $"Common/CourseExtension{i + 1}"
                });
            }

            this._modelBuilder.Entity<CourseExtension>().HasData(courseExtensions);
        }
        public async Task SeedDefaultData()
        {
            //seedRoles
            var systemAdmin = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "System Admin",
                NormalizedName = RoleNormalizedName.SystemAdmin,
                ConcurrencyStamp = ""
            };
            var courseAdmin = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "Course Admin",
                NormalizedName = RoleNormalizedName.CourseAdmin,
                ConcurrencyStamp = ""
            };
            var gSA = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "GSA",
                NormalizedName = RoleNormalizedName.GSA,
                ConcurrencyStamp = ""
            };
            var golfer = new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "Golfer",
                NormalizedName = RoleNormalizedName.Golfer,
                ConcurrencyStamp = ""
            };
            this._modelBuilder.Entity<IdentityRole<Guid>>()
                .HasData(
                systemAdmin,
                courseAdmin,
                gSA,
                golfer
                );
            //seeđAdmin
            var admin = new Golfer();

            admin.Id = Guid.NewGuid();
            admin.UserName = "admin@gmail.com";
            admin.FirstName = "admin";
            admin.LastName = "admin";
            admin.Avatar = "";
            admin.Cover = "";
            admin.Handicap = 0;
            admin.StartHandicap = 0;
            admin.IDX = 0.0;
            
            
            var userRole = new IdentityUserRole<Guid>()
            {
                RoleId =systemAdmin.Id,
                UserId = admin.Id
            };
            var profile = new Profile
            {
                ID = admin.Id,
                ShirtSize = -1,
                ShoesSize = -1,
                PantsSize = -1
            };
            //this._modelBuilder.Entity<IdentityUser<Guid>>()
            //.HasData(admin);
            //this._modelBuilder.Entity<IdentityUserRole<Guid>>()
            //    .HasData(userRole);
            //this._modelBuilder.Entity<Profile>()
            //    .HasData(profile);
            //seedCourseExtenstion
            var courseExtensions = new List<CourseExtension>();
            string[] courseExtensionNames = {
                "Hire caddy",
                "Rent golf cart",
                "Driving range",
                "Pro shop",
                "Rent clubs",
                "Rent shoes",
                "Rent umbrella",
                "Locker",
                "Caddy",
                "Golf cart",
                "Golf cart surcharge",
                "Club house",
                "Restaurant",
                "Palace"
            };

            for (int i = 0; i < courseExtensionNames.Length; i++)
            {
                courseExtensions.Add(new CourseExtension
                {
                    ID = i + 1,
                    Name = courseExtensionNames[i],
                    Icon = $"Common/CourseExtension{i + 1}"
                });
            }

            this._modelBuilder.Entity<CourseExtension>().HasData(courseExtensions);
            //seedSystemSetting
            var sysSetting = new SystemSetting();
            this._modelBuilder.Entity<SystemSetting>().HasData(sysSetting);

        }

    }
}