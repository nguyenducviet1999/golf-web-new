using Golf.Domain.Bookings;
using Golf.Domain.GolferData;
using Golf.HttpApi.Host.Helpers;
using Golf.Services.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/<ProductsController>
        [HttpGet("{productID}")]
        async public Task<ActionResult> Get(Guid productID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _productService.Get(productID);
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Exception.Message);
            }
        }

        /// <summary>
        /// Đăng giờ chơi cho sân
        /// </summary>
        /// <param name="product">Dữ liệu giờ chơi </param>
        /// <returns></returns>
        [HttpPost]
        async public Task<ActionResult> Post([FromBody] Product product)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _productService.Add(golfer.Id, product);
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result);
            }
            else return BadRequest(result.Exception.Message);

        }

        /// <summary>
        /// Sửa giờ chơi sân
        /// </summary>
        /// <param name="product">Dữ liệu sửa đổi</param>
        /// <param name="productID">ĐỊnh danh giờ chơi</param>
        /// <returns></returns>
        // PUT api/<PostController>/5
        [HttpPut("{productID}")]
        async public Task<ActionResult> Put([FromBody] Product product, Guid productID)
        {
            product.ID = productID;
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _productService.Edit(golfer.Id, product);
            if (result.IsCompletedSuccessfully)
                return Ok(result);
            else return BadRequest(result.Exception.Message);
        }

        // DELETE api/<PostController>/5
        /// <summary>
        /// Xóa giờ chơi
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [HttpDelete("{productID}")]
        async public Task<ActionResult> Delete(Guid productID)
        {
            var golfer = (Golfer)HttpContext.Items["Golfer"];
            var result = _productService.Delete(golfer.Id, productID);
            if (result.IsCompletedSuccessfully)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Exception.Message);
            }

        }
    }
}
