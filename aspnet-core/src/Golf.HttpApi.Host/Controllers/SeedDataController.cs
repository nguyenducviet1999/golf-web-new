using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Golf.HttpApi.Host.Helpers;
using Golf.Domain.GolferData;
using Golf.Domain.Courses;
using Golf.Domain.Scorecard;
using Golf.Services;
using Golf.Core.Exceptions;
using Golf.Domain.Shared.Resources;

using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedDataController : ControllerBase
    {
        private readonly SeedDataService _seedDataService;

        public SeedDataController(SeedDataService seedDataService)
        {
            _seedDataService = seedDataService;
        }

        // POST api/SeedDataController/Location
        [HttpPost("Location")]
        public bool SeedLocation()
        {
            return _seedDataService.SeedLocation();
        }

        // POST api/SeedDataController/Course
        [HttpPost("Course")]
        public bool SeedCourse()
        {
            return _seedDataService.SeedCourse();
        }

        // POST api/SeedDataController/Scorecard
        [HttpPost("Scorecard")]
        public bool SeedScorecard()
        {
            return _seedDataService.SeedScorecard();
        }

        [HttpPost("Account")]
        public bool SeedAccount()
        {
            return _seedDataService.SeedAccountAsync().Result;
        }

        [HttpPost("Product")]
        public bool SeedProduct()
        {
            return _seedDataService.SeedProduct();
        }
    }
}