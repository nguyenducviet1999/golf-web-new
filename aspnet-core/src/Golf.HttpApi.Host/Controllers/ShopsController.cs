using AutoMapper;
using Golf.Core.Dtos.Controllers.ShopControler.Request;
using Golf.Core.Dtos.Controllers.ShopControler.Response;
using Golf.Domain.Shared;
using Golf.HttpApi.Host.Helpers;
using Golf.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Golf.HttpApi.Host.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ShopsController : ControllerBase
    {
        private readonly ShopService _shopService;
        private readonly IMapper _mapper;

        public ShopsController(IMapper mapper, ShopService shopService)
        {
            _mapper = mapper;
            _shopService = shopService;

            // Request.Headers.
            //_shopService._odooAPIService.Cookies.Add(new Cookie("session_id", Request.Cookies["MyTestCookie"]));
        }
        // GET: api/<ShopsController>
        /// <summary>
        /// Lọc danh sách sản phẩm
        /// </summary>
        /// <param name="filter">Thể loại(nếu không lọc thì truyền giá trị âm sẽ trả về alll)</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>Danh sách sản phẩm</returns>
        [HttpGet("products/{filter}/{startIndex}")]
        public ActionResult<List<OdooProductResponse>> Get(int filter, int startIndex)
        {
            return Ok(_shopService.GetOdooProductResponses(filter, startIndex).Result);
        }
        /// <summary>
        /// Lấy sanh sách loại sản phẩm
        /// </summary>
        /// <returns></returns>
        [HttpGet("products/category")]
        public ActionResult<List<OdooCategoryResponse>> Get()
        {
            return Ok(_shopService.GetOdooProductCategorys().Result);
        }
        /// <summary>
        /// Lấy chi tiết sản phẩm
        /// </summary>
        /// <param name="productID">Định danh sản phẩm</param>
        /// <returns></returns>
        [HttpGet("products/{productID}")]
        public ActionResult<List<OdooPrductDetailResponse>> GetProductDetail(int productID)
        {
            return Ok(_shopService.GetOdooProductDetail(productID).Result);
        }
        /// <summary>
        /// Láy sanh sách đánh giá sản phẩm
        /// </summary>
        /// <param name="productID">Định danh sản phẩm</param>
        /// <param name="startIndex">vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("products/{productID}/reviews/{startIndex}")]
        public ActionResult<List<OdooProductReviewResponse>> GetProductReview(int productID, int startIndex)
        {
            return Ok(_shopService.GetOdooProductReview(productID, startIndex).Result.Messages);
        }
        /// <summary>
        /// Tạo đánh giá sản phẩm
        /// </summary>
        /// <param name="productID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("products/{productID}/reviews")]
        public ActionResult<bool> AddProductReview(int productID,[FromForm] OdooProductReviewRequest request)
        {
            return Ok(_shopService.AddOdooProductReview(productID, request).Result);
        }
        [HttpPost("Images/Remove")]
        public ActionResult<bool> RemoveOdooResImage(OdooImageRemoveRequest request)
        {
            return Ok(_shopService.RemoveOdooResImage(request.ToOdooImageRemoveRequestDto()).Result);
        }
        //booking
        [HttpPost("CourseBooking/Submit")]
        public async Task<ActionResult<bool>> SubmitCourseBoooking([FromBody] OdooCourseBookingRequest request)
        {
            return Ok(await _shopService.CourseBooking(request));
        }
        //cart
        /// <summary>
        /// Lấy dữ liệu giỏ hàng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Cart")]
        public async Task<ActionResult<OdooCartResponse>> GetCart()
        {
            return Ok(await _shopService.GetCartDetail());
        }
        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        /// <param name="request">Dữ liệu đầu vào(Định danh, Số lượng). Dịnh danh của product sẽ lấy trong api lấy sản phẩm chi tiết trường productVariants</param>
        /// <returns></returns>
        [HttpPost("Cart/UpdateProduct")]
        public async Task<ActionResult<bool>> AddtoCart(OdooAddToCartRequest request)
        {
            return Ok(await _shopService.AddToCart(request.ToOdooCartRequestDto()));
        }
        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng
        /// </summary>
        /// <param name="request">Dữ liệu đầu vào</param>
        /// <returns></returns>
        //[HttpPost("Cart/RemoveProduct")]
        //public async Task<ActionResult<bool>> RemoveFromCart(OdooRemoveFromCartRequest request)
        //{
        //    return Ok(await _shopService.RemoveFromCart(request.ToRemoveFromCartRequestDto()));
        //}
        /// <summary>
        /// submit giở hàng
        /// </summary>
        /// <returns></returns>
        [HttpPost("Cart/Submit")]
        public async Task<ActionResult<bool>> SubmitCart()
        {
            return Ok(await _shopService.SubmitCart());
        }

        //promotion
        /// <summary>
        /// Lấy chi tiết mã giảm giá
        /// </summary>
        /// <param name="promoID"></param>
        /// <returns></returns>
        [HttpGet("PromotionCode/{promoID}")]
        public async Task<ActionResult<OdooPromotionCodeDetailResponse>> GetPromotionDetail(int promoID)
        {
            return Ok(await _shopService.GetOdooPromotionCodeDetail(promoID));
        }
        /// <summary>
        /// Lấy danh sách mã giảm giá
        /// </summary>
        /// <returns></returns>
        [HttpGet("PromotionCode/List")]
        public async Task<ActionResult<List<OdooPromotionCodeDetailResponse>>> GetPromotionDetail()
        {
            return Ok(await _shopService.GetOdooPromotionCodes());
        }
        /// <summary>
        /// Chọn mã giảm giá
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        [HttpPost("PromotionCode/Choose")]
        public async Task<ActionResult<bool>> ChoosePromotionDetail(string promotionCode)
        {
            OdooPromotionCodeRequest request = new OdooPromotionCodeRequest(promotionCode);
            return Ok(await _shopService.ChooseOdooPromotionCode(request));
        }
        /// <summary>
        /// Bỏ chọn mã giảm giá
        /// </summary>
        /// <param name="promotionCode"></param>
        /// <returns></returns>
        [HttpPost("PromotionCode/Unapply")]
        public async Task<ActionResult<bool>> UnapplyPromotionDetail(string promotionCode)
        {
            OdooPromotionCodeRequest request = new OdooPromotionCodeRequest(promotionCode);
            return Ok(await _shopService.UnapplyOdooPromotionCode(request));
        }
        //wish List
        /// <summary>
        /// Theemsarn phẩm yêu thích
        /// </summary>
        /// <param name="productID">Định danh sản phẩm</param>
        /// <returns></returns>
        [HttpPost("WishList/Add/{productID}")]
        public ActionResult<bool> AddWishList(int productID)
        {
            return Ok(_shopService.AddWishList(productID).Result);
        }
        [HttpPost("WishList/Remove/{wishID}")]
        public ActionResult<bool> RemoveWishList(int wishID)
        {
            return Ok(_shopService.RemoveWishList(wishID).Result);
        }
        [HttpGet("WishList")]
        public ActionResult<List<OdooWishListProductResponse>> GetWishList(int startIndex,string searchKey)
        {
            return Ok(_shopService.GetWishList(startIndex,Const.PageSize, searchKey==null?"":searchKey).Result);
        }
        //address
        /// <summary>
        /// Thêm địa chỉ giao hàng
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Address")]
        public ActionResult<string> AddAddress(OdooAddressRequest request)
        {
            var requestDto = _mapper.Map<OdooAddressRequestDto>(request);
            requestDto.ID = -1;
            return Ok(_shopService.Address(requestDto).Result);
        }
        /// <summary>
        /// Sửa địa chỉ giao hàng
        /// </summary>
        /// <param name="iD"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("Address/{iD}")]
        public ActionResult<bool> UpdateAddress(int iD, OdooAddressRequest request)
        {
            var requestDto = _mapper.Map<OdooAddressRequestDto>(request);
            requestDto.ID = iD;
            return Ok(_shopService.Address(requestDto).Result);
        }
        /// <summary>
        /// Chọn địa chỉ giao hàng
        /// </summary>
        /// <param name="iD"></param>
        /// <returns></returns>
        [HttpPut("ChooseAddress/{iD}")]
        public ActionResult<bool> UpdateAddress(int iD)
        {
            return Ok(_shopService.ChooseAddress(new OdooChooseAddressDto(iD)).Result);
        }
        /// <summary>
        /// Lấy danh sách địa chhir giao hàng
        /// </summary>
        /// <param name="startIndex">vị trí phân trang</param>
        /// <returns></returns>
        [HttpGet("MyAdress/{startIndex}")]
        public ActionResult<List<OdooAdressResponse>> MyAddress(int startIndex)
        {
            return Ok(_mapper.Map<List<OdooAdressResponse>>(_shopService.MyAddress(startIndex).Result));
        }

    }
}
