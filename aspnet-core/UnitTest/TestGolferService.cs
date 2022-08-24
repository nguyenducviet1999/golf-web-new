using Golf;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services;
using Golf.Services.Bookings;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    class TestGolferService
    {
        private GolferService _golferService;
        private ProfileService _profileService;
        private RelationshipService _relationshipService;
        [SetUp]
        public void Setup()
        {
            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();
            var serviceScope = webHost.Services.CreateScope();
            _golferService = serviceScope.ServiceProvider.GetService<GolferService>();
            _profileService = serviceScope.ServiceProvider.GetService<ProfileService>();
            _relationshipService = serviceScope.ServiceProvider.GetService<RelationshipService>();
        }
        [Test]
        public void TestSearchGolferByName()
        {
            var reusult = _golferService.SearchGolferByName(new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),"viet", 0).Result.First();
            Assert.AreEqual(reusult.Golfer.FullName,"Viet Nguyen");
        }
        [Test]
        public void TestGetMinimizedGolfer()
        {
            var reusult = _golferService.GetMinimizedGolfer(new Guid("0dc6cf7e-cbf7-4b71-a140-4c2b277e8e44"));
            Assert.AreEqual(reusult.FullName, "Viet Nguyen");
        }
        public void TestGetGolfer()
        {
            var reusult = _golferService.GetGolfer(new Guid("0dc6cf7e-cbf7-4b71-a140-4c2b277e8e44"));
            Assert.Equals(reusult.Handicap, 32);
        }
    }
}
