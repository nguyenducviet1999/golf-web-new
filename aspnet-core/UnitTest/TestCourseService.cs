using Golf;
using Golf.Domain.Common;
using Golf.Services.Bookings;
using Golf.Services.Courses;
using Golf.Services.Locations;
using Golf.Services.Products;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace UnitTest
{
    public class TestCourseService
    {
        private CourseService _courseService;
        [SetUp]
        public void Setup()
        {
            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();
            var serviceScope = webHost.Services.CreateScope();
            _courseService = serviceScope.ServiceProvider.GetService<CourseService>();
        }

        [Test]
        public void TestGetMaxPromotionProductInDate()
        {
            var reusult = _courseService.GetMaxPromotionProductInDate(new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"), DateTime.Now.AddDays(1));
            Assert.AreEqual(reusult.Result.Promotion, 0.45);
        }
        [Test]
        public void TestSearchByName()
        {
            var reusult = _courseService.SearchByName("van tri golf",0,null);
            Assert.AreEqual(reusult.First().Name, "Van Tri Golf Club");
        }
        //[Test]
        //public void TestGetDistance()
        //{
        //    var reusult = _courseService.GetDistance(new GPSAddress() {Longitude= "21.0291298", Latitude = "105.8086038" },new GPSAddress() { Longitude = "21.0291298", Latitude = "105.9086038" });
        //    Assert.AreEqual(int.Parse(reusult),13 );
        //}
    }
}