// using Golf.Domain.Courses;
// using Golf.Domain.Golfer;
// using Golf.Services.Courses;
// using Golf.Services.Products;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// // For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// namespace Golf.HttpApi.Host.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class CoursesController : ControllerBase
//     {
//         ProductService _productService;
//         CourseService _courseService;
//         CourseReviewService _courseReviewService;
//         public CoursesController(CourseService courseService, ProductService productService, CourseReviewService courseReviewService)
//         {
//             _courseService = courseService;
//             _productService = productService;
//             _courseReviewService = courseReviewService;
//         }

//         [HttpGet("booking/{filter}/{startIndex}")]
//         async public Task<ActionResult> GetBookingLocation(int startIndex, int filter, [FromBody] DateTime date)
//         {
//             var golfer = (Golfer)HttpContext.Items["Golfer"];
//             var result = _courseService.GetCourseBookingByFilter(golfer.Id, startIndex, filter, date);

//             if (result.IsCompletedSuccessfully)
//             {
//                 return Ok(result);
//             }
//             else return BadRequest(result.Exception.Message);
//         }

//         [HttpGet("{id}/products/{startIndex}")]
//         async public Task<ActionResult> GetCourseProduct(Guid id, [FromBody] DateTime dateTime, int startIndex)
//         {

//             var result = _productService.GetProductByCourseID(id, dateTime, startIndex);
//             if (result.IsCompletedSuccessfully)
//             {
//                 return Ok(result);
//             }
//             else
//             {
//                 return BadRequest(result.Exception.Message);
//             }
//         }
//     }
// }
