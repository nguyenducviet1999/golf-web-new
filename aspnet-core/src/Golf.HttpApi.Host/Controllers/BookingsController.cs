
using Golf.Core.Dtos.Controllers.BookingController.Responses;
using Golf.Core.Dtos.Controllers.TransactionController.Requests;
using Golf.Domain.Bookings;
using Golf.Domain.Common;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.Bookings;
using Golf.Services.Courses;
using Golf.Services.Locations;
using Golf.Services.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly LocationService _locationService;
        private readonly CourseService _courseService;
        private readonly ProductService _productService;
        private readonly TransactionService _transactionService;

        public BookingsController(TransactionService transactionService, ProductService productService, LocationService locationService, CourseService courseService)
        {
            _productService = productService;
            _courseService = courseService;
            _locationService = locationService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Lấy danh sách các sân theo tiêu chí lọc nào đó(filter)
        /// </summary>
        /// <param name="index">Số thứ tự phần tử dầu tiên</param>
        /// <param name="filter">Giá trị lọc public const int Promotion = 1, Favorite = 2, Nearest = 3, Weekend = 4 </param>
        /// <param name="date">Thời điểm </param>
        /// <returns> Danh sách các sân</returns>
        [HttpGet("{filter}/{index}/{date}")]
        public ActionResult<List<BookingResponse>> GetBookingCourseFillter(int index, int filter, DateTime date, [FromQuery] GPSAddressDto gPSAddress)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _courseService.GetBookingCourseByFilter(golfer.Id, index, filter, date, gPSAddress.GPSAddress());

            if (result.Result != null)
            {
                return Ok(result.Result.ToList());
            }
            else return new List<BookingResponse>();
        }

        /// <summary>
        /// Tìm kiếm sân hoặc địa điểm theo tên
        /// </summary>
        /// <param name="searchKey">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách vị trí và danh sách sân</returns>
        [HttpGet("Course/search/{searchKey}")]
        public ActionResult<SearchBookingResponse> searchCourse(string searchKey, [FromQuery] GPSAddressDto gPSAddress)
        {
            return _locationService.SearchLocationByName(searchKey==null?"":searchKey.ToLower().Trim(), gPSAddress.GPSAddress());
        }

        /// <summary>
        /// Lấy danh sách location booking(danh sách hiện đầu tiên khi vào booking)
        /// </summary>
        /// <param name="startIndex">Thứ tự phần tử dầu tiên lấy</param>
        /// <returns>Danh sách các location</returns>
        [HttpGet("{startIndex}")]
        public ActionResult<List<BookingResponse>> GetBookingLocation(int startIndex, [FromQuery] GPSAddressDto gPSAddress)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _locationService.GetBookingLocation(golfer.Id, startIndex, gPSAddress.GPSAddress());
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result.Result);
            }
            else return BadRequest(result.Exception.Message);
        }

        /// <summary>
        /// Lấy thông tin sân hiển thị trong màn hình đặt hàng
        /// </summary>
        /// <param name="courseID">ID sân golf</param>
        /// <param name="date">Thời điểm </param>
        /// <returns>Dữ liệu sân và thông tin khuyến mại trong 14 ngày kế tiếp kể từ thời điểm truyền vào</returns>
        [HttpGet("course/{courseID}/{date}")]
        public ActionResult<CourseBookingResponse> GetCoureBookingByID(Guid courseID, DateTime date, [FromQuery] GPSAddressDto gPSAddress)
        {
            CourseBookingResponse courseBookingResponse = new CourseBookingResponse();
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _courseService.GetCourseBookingByID(golfer.Id, courseID, date, gPSAddress.GPSAddress());
            courseBookingResponse.CourseInfor = result.Result;
            for (int i = 0; i < 14; i++)
            {
                DateBookingInfo dateBookingInfo = new DateBookingInfo();
                dateBookingInfo.Date = date.Date;
                dateBookingInfo.DayOfWeek = date.DayOfWeek.ToString();
                dateBookingInfo.IsPromotion = _courseService.IsPromotion(courseID, date);
                courseBookingResponse.DateBookingInfos.Add(dateBookingInfo);
                date = date.AddDays(1);
            }
            if (result.IsCompletedSuccessfully)
            {
                return Ok(courseBookingResponse);
            }
            else return BadRequest(result.Exception.Message);
        }

        /// <summary>
        /// Lấy danh sách sân của location
        /// </summary>
        /// <param name="locationId">ID của location</param>
        /// <param name="index">Số thứ tự phần tử đầu tiên</param>
        /// <param name="gPSAddress">Tọa độ GPS người dùng</param>
        /// <returns>Danh sách sân</returns>
        [HttpGet("Location/{locationId}/courses/{index}")]
        public ActionResult<List<BookingResponse>> GetCourseOfLocation(Guid locationId, int index, [FromQuery] GPSAddressDto gPSAddress)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _locationService.GetBookingCourseByLocationId(golfer.Id, locationId, index, gPSAddress.GPSAddress());
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result.Result.ToList());
            }
            else
            {
                return BadRequest(result.Exception.Message);
            }
        }

        /// <summary>
        /// Lấy các product của sân theo ngày
        /// </summary>
        /// <param name="courseId">ID sân</param>
        /// <param name="dateTime">Ngày muốn đặt giờ chơi</param>
        /// <returns>Danh sách giờ chơi</returns>
        [HttpGet("course/{courseId}/products/{dateTime}")]
        public ActionResult<CourseProductResponses> GetCourseProduct(Guid courseId, DateTime dateTime)
        {
            var result = _productService.GetProductResponeByCourseID(courseId, dateTime);
            if (result.Result != null)
            {
                return Ok(result.Result);
            }
            else
            {
                return BadRequest(result.Exception.Message);
            }
        }

        /// <summary>
        /// Gửi yêu cầu đặt giờ chơi
        /// </summary>
        /// <param name="transactionRequest">Dữ liệu đầu vào</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<bool> AddRequestTrnsaction([FromBody] TransactionRequest transactionRequest)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            transactionRequest.GolferID = golfer.Id;
            var result = _transactionService.AddTransactionRequest(transactionRequest);
            return Ok(result);
        }

    }
}
