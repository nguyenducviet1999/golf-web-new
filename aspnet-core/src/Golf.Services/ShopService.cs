using AutoMapper;
using Golf.Core.Common.Odoo.OdooResponse;
using Golf.Core.Dtos.Controllers.ShopControler.Request;
using Golf.Core.Dtos.Controllers.ShopControler.Response;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Domain.Shared;
using Golf.Domain.Shared.OdooAPI;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.OdooAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services
{
    public class ShopService
    {
        private readonly UserManager<Golfer> _golferManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ProfileRepository _profileRepository;
        private readonly SignInManager<Golfer> _signInManager;
        private const string GoogleApiTokenInfoUrl = "https://oauth2.googleapis.com/tokeninfo?id_token={0}";
        private readonly DatabaseTransaction _databaseTransaction;
        public OdooAPIService _odooAPIService;
        private readonly IMapper _mapper;

        public ShopService(
            IMapper mapper,
            OdooAPIService odooAPIService,
            UserManager<Golfer> golferManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ProfileRepository profileRepository,
            SignInManager<Golfer> signInManager,
            DatabaseTransaction databaseTransaction)
        {
            _mapper = mapper;
            _odooAPIService = odooAPIService;
            _golferManager = golferManager;
            _roleManager = roleManager;
            _profileRepository = profileRepository;
            _signInManager = signInManager;
            _databaseTransaction = databaseTransaction;
        }


        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        /// <param name="filter">giá trị lọc</param>
        /// <param name="startIndex">vị trí phân trang</param>
        /// <returns></returns>
        public async Task<List<OdooProductResponse>> GetOdooProductResponses(int filter, int startIndex)
        {
            OdooGetProductsRequestDto odooGetProductsRequest = new OdooGetProductsRequestDto(filter, startIndex);
            var result = await _odooAPIService.CallAPI<OdooGetProductsRequestDto, OdooResponse<OdooResult<List<OdooProductResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getProductTemplateUrl(), odooGetProductsRequest);
            return _mapper.Map<List<OdooProductResponse>>(result.Result.Data);
        }
        /// <summary>
        /// Lấy danh mục loại sản sản phảm
        /// </summary>
        /// <returns></returns>
        public async Task<List<OdooCategoryResponse>> GetOdooProductCategorys()
        {
            OdooGetCategorysRequestĐto odooGetCategorysRequest = new OdooGetCategorysRequestĐto();
            var result = await (_odooAPIService.CallAPI<OdooGetCategorysRequestĐto, OdooResponse<OdooResult<List<OdooCategoryResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getProductCategoryUrl(), odooGetCategorysRequest));
            return _mapper.Map<List<OdooCategoryResponse>>(result.Result.Data);
        }
        /// <summary>
        /// Lấy chi tiết sản phẩm
        /// </summary>
        /// <param name="productID">Định danh sản phẩm</param>
        /// <returns>Dữ liệu chi tiết sản phẩm</returns>
        public async Task<OdooPrductDetailResponse> GetOdooProductDetail(int productID)
        {
            var wishList = await this.GetWishList(0, null, "");
            var wishListID = wishList.Select(wl => wl.ProductID).ToList();
            var result = await (_odooAPIService.CallAPI<int, OdooResult<OdooPrductDetailResponseDto>>(APIMethod.GET, _odooAPIService._appSettings.getProductDetail(), productID));
            var response= _mapper.Map<OdooPrductDetailResponse>(result.Data);
            foreach(var i in response.ProductVariants)
            {
                if(wishListID.Contains(i.ID))
                {
                    i.IsFavourite = true;
                }    
            }
            return response;
        }
        //product review
        public async Task<OdooProductReviewResponses> GetOdooProductReview(int productID, int startIndex)
        {
            OdooGetProductReviewRequestDto request = new OdooGetProductReviewRequestDto(productID, startIndex);
            var result = await (_odooAPIService.CallAPI<OdooGetProductReviewRequestDto, OdooResponse<OdooProductReviewResponsesDto>>(APIMethod.POST, _odooAPIService._appSettings.getProductReview(), request));
            var response = _mapper.Map<OdooProductReviewResponses>(result.Result);
            foreach (var i in response.Messages)
            {
                i.Body = i.Body.Replace("<p>", "").Replace("</p>", "");
            }
            return response;
        }
        public async Task<bool> AddOdooProductReview(int productID, OdooProductReviewRequest request)
        {
            List<string> imageIDs = new List<string>();
            if (request.Images != null)
            {
                foreach (var i in request.Images)
                {
                    var tmp = await AddOdooResImage(new OdooImageAddRequest() { ResID = productID.ToString(), ImageFile = i });
                    imageIDs.Add(tmp.Data.ID.ToString());
                }
            }
            var result = await (_odooAPIService.CallAPI<OdooProductReviewRequestDto, OdooResponse<DataResponseBase>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.AddProductReview), request.ToOdooProductReviewRequestDto(productID, imageIDs)));
            return true;
        }
        public async Task<OdooResult<OdooImageResponseDto>> AddOdooResImage(OdooImageAddRequest request)
        {
            var result = await (_odooAPIService.CallAPI<OdooResult<OdooImageResponseDto>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.AddProductReviewImage), request.FormData()));
            return _mapper.Map<OdooResult<OdooImageResponseDto>>(result);
        }
        public async Task<bool> RemoveOdooResImage(OdooImageRemoveRequestDto request)
        {
            var result = await (_odooAPIService.CallAPI<OdooImageRemoveRequestDto, OdooResponse<OdooResult<dynamic>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.RemoveProductReviewImage), request));
            return true;

        }
        //booking
        
            public async Task<bool> CourseBooking(OdooCourseBookingRequest request)
        {
            var result = await (_odooAPIService.CallAPI<OdooCourseBookingRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.CourseBooking), request.ToOdooCourseBookingRequestDto(_odooAPIService.GetPartnerID())));
            return true;

        }
        //promotion
        public async Task<OdooPromotionCodeDetailResponse> GetOdooPromotionCodeDetail(int promoID)
        {
            var result = await (_odooAPIService.CallAPI<Object, OdooResponse<OdooResult<List<OdooPromotionCodeDetailResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getPromotionCode() + "/" + promoID, new Object()));
            return _mapper.Map<OdooPromotionCodeDetailResponse>(result.Result.Data.First());
        }
        public async Task<List<OdooPromotionCodeDetailResponse>> GetOdooPromotionCodes()
        {
            var result = await (_odooAPIService.CallAPI<string, OdooResult<List<OdooPromotionCodeResponseDto>>>(APIMethod.GET, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ListPromotionCode), null));
            return _mapper.Map<List<OdooPromotionCodeDetailResponse>>(result.Data);
        }
        public async Task<bool> ChooseOdooPromotionCode(OdooPromotionCodeRequest request)
        {
            var result = await (_odooAPIService.CallAPI<OdooPromotionCodeRequest, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ChoosePromotionCode), request));
            return true;
        }
        public async Task<bool> UnapplyOdooPromotionCode(OdooPromotionCodeRequest request)
        {
            var result = await (_odooAPIService.CallAPI<OdooPromotionCodeRequest, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.UnapplyPromotionCode), request));
            return true;
        }
        //cart
        public async Task<OdooCartResponse> GetCartDetail()
        {
            var result = await (_odooAPIService.CallAPI<string, OdooResult<OdooCartResponseDto>>(APIMethod.GET, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.Cart), null));
            return _mapper.Map<OdooCartResponse>(result.Data);
        }
        public async Task<bool> AddToCart(OdooAddToCartRequestDto request)
        {
            var result = await (_odooAPIService.CallAPI<OdooAddToCartRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.AddToCart), request));
            return true;
        }
        public async Task<bool> RemoveFromCart(OdooRemoveFromCartRequestDto request)
        {
            var result = await (_odooAPIService.CallAPI<OdooRemoveFromCartRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.RemoveFromCart), request));
            return true;
        }
        public async Task<bool> SubmitCart()
        {
            var result = await (_odooAPIService.CallAPI<Object, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.SubmitCart), new Object()));
            return true;
        }

        //wishlist
        public async Task<bool> AddWishList(int productID)
        {
            OdooAddWishListRequestDto request = new OdooAddWishListRequestDto(productID);
            var result = await (_odooAPIService.CallAPI<OdooAddWishListRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.WishListAdd), request));
            return true;
        }
        public async Task<bool> RemoveWishList(int wishID)
        {
            OdooRemoveWishListRequestDto request = new OdooRemoveWishListRequestDto(wishID);
            var result = await (_odooAPIService.CallAPI<OdooRemoveWishListRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.WishListRemove), request));
            return true;
        }
        public async Task<List<OdooWishListProductResponse>> GetWishList(int startIndex, int? pageSize, string searchKey)
        {
            OdooWishListRequestDto request = new OdooWishListRequestDto(startIndex, pageSize, searchKey);
            var result = await (_odooAPIService.CallAPI<OdooWishListRequestDto, OdooResponse<OdooResult<List<OdooWishListProductResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.WishList), request));
            return _mapper.Map<List<OdooWishListProductResponse>>(result.Result.Data);
        }
     
        //address
        /// <summary>
        /// thêm, sửa địa chỉ.(ID=-1 thêm)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> Address(OdooAddressRequestDto request)
        {
            var result = await (_odooAPIService.CallAPI<OdooAddressRequestDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.Address), request));
            return true;
        }
        public async Task<bool> ChooseAddress(OdooChooseAddressDto request)
        {
            var result = await (_odooAPIService.CallAPI<OdooChooseAddressDto, OdooResponse<OdooResult<DataResponseBase>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.ChooseAddress), request));
            return true;
        }
        public async Task<List<OdooAdressResponseDto>> MyAddress(int startIndex)
        {
            OdooGetMyAddressRequestDto request = new OdooGetMyAddressRequestDto(_odooAPIService.GetPartnerID(), startIndex);
            var result = await (_odooAPIService.CallAPI<OdooGetMyAddressRequestDto, OdooResponse<OdooResult<List<OdooAdressResponseDto>>>>(APIMethod.POST, _odooAPIService._appSettings.getUrl(_odooAPIService._appSettings.MyAddress), request));
            return result.Result.Data;
        }


    }
}
