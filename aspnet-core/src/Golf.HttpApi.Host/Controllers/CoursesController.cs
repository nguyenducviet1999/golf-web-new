using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

using Golf.Domain.Shared.Course;
using Golf.HttpApi.Host.Helpers;
using Golf.Core.Common.Course;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;

using Golf.Core.Dtos.Controllers.CoursesController.Requests;
using Golf.Core.Dtos.Controllers.CoursesController.Responses;
using Golf.Services.Courses;
using Golf.Domain.Common;
using System.Threading.Tasks;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared;
using Golf.Services.Bookings;
using System.Linq;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseService _courseService;
        private readonly CourseReviewRepository  _courseReviewRepository;
        private readonly TransactionRepository  _transactionRepository;

        public CoursesController(TransactionRepository transactionRepository, CourseReviewRepository courseReviewRepository, CourseService courseService)
        {
            _transactionRepository = transactionRepository;
            _courseReviewRepository = courseReviewRepository;
            _courseService = courseService;
        }

        /// <summary>
        /// Lấy đanh sách đánh giá sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        [HttpGet("{CourseID}/Review/{startIndex}")]
        public async Task<ActionResult<CourseReviewResponses>> GetCourseReviews(Guid CourseID,int startIndex)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            CourseReviewResponses courseReviewResponses = new CourseReviewResponses();
            var results = _transactionRepository.FindWithProduct(t => t.GolferID == currentGolfer.Id &&t.Product.CourseID==CourseID && t.Status == TransactionStatus.Completed).ToList();
            if (results.Count() > 0) courseReviewResponses.AllowReview = true;
            courseReviewResponses.ListCourseReviewResponse = _courseService.GetCourseReviews(CourseID, startIndex, Const.PageSize);
            return Ok(courseReviewResponses);
        }   
        /// <summary>
        /// Gửi review course
        /// </summary>
        /// <param name="CourseID">ID Course</param>
        /// <param name="request">Dữ liệu review</param>
        /// <returns></returns>
        [HttpPost("{CourseID}/Review")]
        public async Task<ActionResult<CourseReviewResponse>> AddCourseReview(Guid CourseID,[FromForm] AddCourseReviewRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            return Ok(await _courseService.AddCourseReview(CourseID, currentGolfer, request));
        }

        /// <summary>
        /// Sửa review sân
        /// </summary>
        /// <param name="CourseID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{CourseID}/Review")]
        public ActionResult EditCourseReview(Guid CourseID, EditCourseReviewRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
             _courseService.EditCourseReview(CourseID, currentGolfer, request);
            return Ok();
        }

        /// <summary>
        /// Xóa review sân
        /// </summary>
        /// <param name="CourseID">id sân</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpDelete("{CourseID}/Review")]
        public ActionResult DeleteCourseReview(Guid CourseID, DeleteCourseReviewRequest request)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            _courseService.DeleteCourseReview(CourseID, currentGolfer, request);
            return Ok();
        }

        /// <summary>
        /// lấy danh sách review sân
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        //[HttpGet("{CourseID}/Reviews")]
        //public ActionResult<GetCourseReviewsResponse> GetCourseReviews(Guid CourseID)
        //{
        //    List<CourseReviewResponse> courseReviewsResponse = _courseService.GetCourseReviews(CourseID);
        //    return Ok(new GetCourseReviewsResponse
        //    {
        //        CourseID = CourseID,
        //        CourseReviews = courseReviewsResponse
        //    });
        //}

        /// <summary>
        /// Lấy Tee của sân
        /// </summary>
        /// <param name="CourseID">ID sân</param>
        /// <returns></returns>
        [HttpGet("{CourseID}/Tees")]
        public ActionResult<GetCourseTeesResponse> GetCourseTees(Guid CourseID)
        {
            List<Tee> courseTees = _courseService.GetCourseTees(CourseID);
            return Ok(new GetCourseTeesResponse
            {
                CourseID = CourseID,
                Tees = courseTees
            });
        }

        /// <summary>
        /// Lấy dữ liệu các hố của sân ứng với các TeeColor
        /// </summary>
        /// <param name="CourseID">ID sân</param>
        /// <param name="color">Màu sắc Tee</param>
        /// <returns></returns>
        [HttpGet("{CourseID}/Tees/{color}/Holes")]
        public ActionResult<GetCourseHolesResponse> GetCourseHoles(Guid CourseID, TeeColor color)
        {
            var courseHoles = _courseService.GetCourseHolesByTee(CourseID, color);
            return Ok(new GetCourseHolesResponse
            {
                CourseID = CourseID,
                CourseHoles = courseHoles
            });
        }

        /// <summary>
        /// Lấy sánh sân 
        /// </summary>
        /// <param name="startIndex">STT sân đầu tiên</param>
        /// <param name="gPSAddress">Tọa độ GPS của người dùng</param>
        /// <returns></returns>
        [HttpGet("GetCourses/{startIndex}")]
        public ActionResult<List<CourseResponse>> GetCourses(int startIndex, [FromQuery] GPSAddressDto gPSAddress)
        {
            var courses = _courseService.GetCourses(startIndex, gPSAddress.GPSAddress());
            return Ok(courses);
        }

        /// <summary>
        /// lấy sân theo ID sân
        /// </summary>
        /// <param name="CourseID"></param>
        /// <param name="gPSAddress"></param>
        /// <returns></returns>
        [HttpGet("{CourseID}")]
        public ActionResult<CourseResponse> GetCourse(Guid CourseID, [FromQuery] GPSAddressDto gPSAddress)
        {
            var course = _courseService.GetCourse(CourseID, gPSAddress.GPSAddress());
            return Ok(course);
        }

        /// <summary>
        /// Tìm sân theo tên 
        /// </summary>
        /// <param name="SearchKey">Từ khóa tìm kiếm</param>
        /// <param name="startIndex"></param>
        /// <param name="gPSAddress"></param>
        /// <returns></returns>
        [HttpGet("searchByName/{startIndex}")]
        public ActionResult<List<CourseResponse>> GetCoursesByName([FromQuery] string searchKey, int startIndex, [FromQuery] GPSAddressDto gPSAddress)
        {
            var courses = _courseService.SearchByName(searchKey == null ? "" : searchKey.Trim().ToLower(), startIndex, gPSAddress.GPSAddress());
            return Ok(courses);
        }

        /// <summary>
        /// Lấy danh sách sân người dùng đã từng chơi và sắp xếp theo thời gian chơi gần nhất
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="gPSAddress">Vị trí hiện tại</param>
        /// <returns></returns>
        [HttpGet("Played/{startIndex}")]
        public ActionResult<List<CourseResponse>> GetCoursesPlayed(int startIndex, [FromQuery] GPSAddressDto gPSAddress)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var courses = _courseService.GetCoursePlayed(currentGolfer.Id, startIndex, gPSAddress.GPSAddress());
            return Ok(courses);
        }
        /// <summary>
        /// Lấy danh sách các sân gần đây
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="gPSAddress">vị trí hiện tại</param>
        /// <returns></returns>
        [HttpGet("Nearest/{startIndex}")]
        public ActionResult<List<CourseResponse>> GetCoursesNearest(int startIndex, [FromQuery] GPSAddressDto gPSAddress)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var courses = _courseService.GetCourseNearest(startIndex, gPSAddress.GPSAddress());
            return Ok(courses);
        }

        /// <summary>
        /// Lấy thông tin chi tiết course 
        /// </summary>
        /// <param name="courseID">Định danh sân</param>
        /// <returns></returns>
        [HttpGet("{courseID}/Detail")]
        public ActionResult<List<CourseDetailResponse>> GetCourseDetail(Guid courseID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            CourseDetailResponse courseDetailResponse = _courseService.GetCourseDetail(golfer.Id, courseID);
            return Ok(courseDetailResponse);
        }
    }
}