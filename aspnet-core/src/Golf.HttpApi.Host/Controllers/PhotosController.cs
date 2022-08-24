using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Golf.Domain.Shared.Resources;
using Golf.Domain.GolferData;
using Golf.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Golf.HttpApi.Host.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly PhotoService _photoService;

        public PhotosController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        // POST api/<PhotosController>
        /// <summary>
        /// Đăng ảnh 
        /// </summary>
        /// <param name="file">FIle dữ liệu ảnh</param>
        /// <returns>Tên ảnh</returns>
        [HttpPost]
        public ActionResult<string> Post([FromForm] PhotoFile file)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var photo = _photoService.SavePhoto(currentGolfer.Id, file.photo, PhotoType.Post);
            if (photo == null)
                return BadRequest("Error save photo");
            else
                return Ok(photo.Result.Name);
        }

        /// <summary>
        /// Xóa ảnh
        /// </summary>
        /// <param name="photoName">Tên ảnh</param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult<string> Delete(string photoName)
        {
            var currentGolfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _photoService.DeletePhoto(currentGolfer.Id, photoName);
            if (result == false)
                return BadRequest("Error save photo");
            else
                return Ok(result);
        }
    }
}

