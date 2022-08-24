using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Golf.HttpApi.Host.Helpers;
using Golf.Domain.Courses;
using Golf.Services;
using Golf.Core.Exceptions;
using Golf.Domain.Shared.Resources;

using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Golf.Services.Bookings;
using Golf.Core.Dtos.Controllers.TransactionController.Respone;
using Golf.Domain.GolferData;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Courses;
using Golf.Services.CourseAdmin;
using Golf.Domain.Shared;
using Golf.Domain.Shared.Transactions;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize(roles: "Course Admin")]
    // [ApiController]
    public class CourseAdminController : ControllerBase
    {
        private readonly CourseAdminService _courseAdminService;
        private readonly TransactionService _transactionService;
        private readonly GolferRepository _golferRepository;
        private readonly CourseRepository _courseRepository;
        private readonly MemberShipService _courseMemberShipService;

        public CourseAdminController(MemberShipService courseMemberShipService, CourseAdminService courseAdminService, TransactionService transactionService, GolferRepository golferRepository, CourseRepository courseRepository)
        {
            _courseAdminService = courseAdminService;
            _transactionService = transactionService;
            _golferRepository = golferRepository;
            _courseRepository = courseRepository;
            _courseMemberShipService = courseMemberShipService;
        }

        // POST api/CourseAdmin/Location
        /// <summary>
        /// Tạo location mới
        /// </summary>
        /// <param name="request">Dữ l</param>
        /// <returns></returns>
        [HttpPost("Location")]
        public ActionResult<Location> CreateLocation(CreateLocationRequest request)
        {
            if (request.Validate())
            {
                var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
                var location = _courseAdminService.AddLocation(currentGolfer, request);
                return Ok(location);
            }
            throw new BadRequestException("Bad Request");
        }

        // POST api/CourseAdmin/Course
        /// <summary>
        /// Thêm sân mới
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Course")]
        public ActionResult<Course> CreateCourse(CreateCourseRequest request)
        {
            if (request.Validate())
            {
                var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
                var course = _courseAdminService.AddCourse(currentGolfer, request);
                return Ok(course);
            }
            throw new BadRequestException("Bad Request");
        }

        // PUT api/CourseAdmin/Course/{CourseID}/Cover
        /// <summary>
        /// Đăng awnrh bìa của sân
        /// </summary>
        /// <param name="CourseID">ID sân</param>
        /// <param name="request">dữ liệu ảnh</param>
        /// <returns></returns>
        [HttpPut("Course/{CourseID}/Cover")]
        public async Task<ActionResult<Course>> SetCourseCover(Guid CourseID, [FromForm] SetCourseCoverRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var course = await _courseAdminService.SetCover(currentGolfer, CourseID, request);
            return Ok(course);
        }

        // PUT api/CourseAdmin/Course/{CourseID}/Photos
        /// <summary>
        /// đăng ảnh của sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="request">Dữ liệu ảnh</param>
        /// <returns></returns>
        [HttpPut("Course/{CourseID}/Photos")]
        public async Task<ActionResult<Course>> EditCoursePhotos(Guid CourseID, [FromForm] EditCoursePhotosRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var course = await _courseAdminService.EditCoursePhotos(currentGolfer, CourseID, request);
            return Ok(course);
        }

        // PUT api/CourseAdmin/Location/{LocationID}
        /// <summary>
        /// Sửa thông tin location 
        /// </summary>
        /// <param name="LocationID">định danh địa điểm</param>
        /// <param name="request">Dữ liệu chỉnh sửa</param>
        /// <returns></returns>
        [HttpPut("Location/{LocationID}")]
        public ActionResult<Location> EditLocation(Guid LocationID, EditLocationRequest request)
        {
            if (request.Validate())
            {
                var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
                var location = _courseAdminService.EditLocation(currentGolfer, LocationID, request);
                return Ok(location);
            }
            throw new BadRequestException("Bad Request");
        }

        // PUT api/CourseAdmin/Course/{CourseID}
        /// <summary>
        /// Sửa thông tin sân
        /// </summary>
        /// <param name="CourseID"> Định danh sân</param>
        /// <param name="request">Dữ liệu chỉnh sửa saan</param>
        /// <returns></returns>
        [HttpPut("Course/{CourseID}")]
        public ActionResult<Course> EditCourse(Guid CourseID, EditCourseRequest request)
        {
            if (request.Validate())
            {
                var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
                var course = _courseAdminService.EditCourse(currentGolfer, CourseID, request);
                return Ok(course);
            }
            throw new BadRequestException("Bad Request");
        }

        /// <summary>
        /// lấy danh sách sân sở hữu
        /// </summary>
        /// <returns>Danh sách sân</returns>
        [HttpGet("Courses")]
        public ActionResult<List<Course>> GetOwnerCourses()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var courses = _courseAdminService.GetOwnerCourses(currentGolfer);
            return Ok(courses);
        }

        /// <summary>
        /// Lấy danh sách sân chờ duyệt
        /// </summary>
        /// <returns>Danh sách sân</returns>
        [HttpGet("Courses/Pending")]
        public ActionResult<List<Course>> GetPendingCourses()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var courses = _courseAdminService.GetOwnerPendingCourses(currentGolfer);
            return Ok(courses);
        }

        /// <summary>
        /// Lấy danh sách địa điểm sở hữu
        /// </summary>
        /// <returns>Danh sách địa điểm</returns>
        [HttpGet("Locations")]
        public ActionResult<List<Location>> GetOwnerLocations()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var locations = _courseAdminService.GetOwnerLocations(currentGolfer);
            return Ok(locations);
        }

        // GET api/CourseAdmin/Locations/Pending
        /// <summary>
        /// Danh sách địa điểm chờ duyệt
        /// </summary>
        /// <returns></returns>
        [HttpGet("Locations/Pending")]
        public ActionResult<List<Location>> GetOwnerPendingLocations()
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var locations = _courseAdminService.GetOwnerPendingLocations(currentGolfer);
            return Ok(locations);
        }

        // DELETE api/CourseAdmin/Course/CourseID
        /// <summary>
        /// Xóa môt sân
        /// </summary>
        /// <param name="CourseID">Đạnh danh sân</param>
        /// <returns></returns>
        [HttpDelete("Course/{CourseID}")]
        public ActionResult<Boolean> DeleteCourse(Guid CourseID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_courseAdminService.DeleteCourse(currentGolfer, CourseID));
        }

        // DELETE api/CourseAdmin/Location/LocationID
        /// <summary>
        /// Xóa một địa điểm 
        /// </summary>
        /// <param name="LocationID">Định danh địa điểm</param>
        /// <returns></returns>
        [HttpDelete("Location/{LocationID}")]
        public ActionResult<Boolean> DeleteLocation(Guid LocationID)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(_courseAdminService.DeleteLocation(currentGolfer, LocationID));
        }

        ///// <summary>
        ///// Thêm thành viên sân
        ///// </summary>
        ///// <param name="courseID">ID sân</param>
        ///// <param name="golferID">ID người dùng</param>
        ///// <returns></returns>
        //[HttpPost("membership/{courseID}/{golferID}")]
        //public ActionResult<Boolean> AddCourseMemberShip(Guid courseID,Guid golferID)
        //{
        //    var golfer = _golferRepository.Get(golferID);
        //    var course = _courseRepository.Get(courseID);
        //    var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
        //    if(course.OwnerID!=currentGolfer.Id)
        //    {
        //        throw new ForbiddenException("This golfer can't");
        //    }    
        //    if (golfer == null)
        //        throw new NotFoundException("Golfer isn't exit!");
        //    if (course == null)
        //        throw new NotFoundException("Course isn't exit!");
        //    return Ok(_courseMemberShipService.AddMemberShip(golferID));
        //}
        //Course Booking

        [HttpGet("Courses/{courseID}/Transactions/{transactionStatus}")]
        public ActionResult<List<TransactionResponse>> GetCourseTransactionRequest(Guid coureID, TransactionStatus transactionStatus, int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var course = _courseAdminService.GetCourse(coureID);
            if (course.OwnerID != currentGolfer.Id)
            {
                return BadRequest("This golfer can't get transaction request");
            }
            var transactions = _transactionService.GetTransactionByCourse(currentGolfer.Id,coureID,transactionStatus,startIndex);
            return Ok(transactions);
        }
        [HttpPut("Transactions/{transactionID}/{transactionStatusRequest}")]
        public ActionResult<List<TransactionResponse>> UpdateTransaction(Guid transactionID,TransactionStatusRequest transactionStatusRequest)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var transactions = _transactionService.UpdateTransaction(currentGolfer.Id, transactionID, transactionStatusRequest);
            return Ok(transactions);
        }
    }
}