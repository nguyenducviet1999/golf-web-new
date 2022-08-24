using AutoMapper;
using Golf.Core.Exceptions;
using Golf.Domain.Shared;
using Golf.Domain.Bookings;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Golf.Core.Dtos.Controllers.BookingController.Responses;
using Golf.Services.Courses;

namespace Golf.Services.Products
{
    public class ProductService
    {
        public readonly ProductRepository _productRepository;
        private readonly CourseRepository _courseRepository;
        private readonly CourseExtensionService _courseExtensionService;
        private readonly IMapper _mapper;

        public ProductService(
            ProductRepository productRepository,
            CourseRepository courseRepository,
            CourseExtensionService courseExtensionService,
            IMapper mapper)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _courseRepository = courseRepository;
            _courseExtensionService = courseExtensionService;
        }
        /// <summary>
        /// thêm một sản phâm
        /// </summary>
        /// <param name="uId">Id người tạo hiện tại</param>
        /// <param name="product">Sản phẩm cần tạo</param>
        /// <returns></returns>
        async public Task<Product> Add(Guid uId, Product product)
        {
            var course = _courseRepository.Get(product.CourseID);
            if (uId == course.CreatedBy || uId == course.OwnerID)
            {
                product.CreatedBy = uId;

                _productRepository.Add(product);
                return _productRepository.Get(product.ID);
            }

            else
            {
                throw new BadRequestException("Can Not add product !");
            }

        }
        /// <summary>
        /// Lấy sản phẩm theo id của nó
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<Product> Get( Guid id)
        {
            var result = _productRepository.Get(id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found group !");
            }


        }
        /// <summary>
        /// Lấy danh sách các sản phẩm của sân theo từng ngày
        /// </summary>
        /// <param name="cId">id sân</param>
        /// <param name="dateTime">ngày </param>
        /// <returns></returns>
        async public Task<CourseProductResponses> GetProductResponeByCourseID(Guid cId, DateTime dateTime)
        {
            CourseProductResponses result = new CourseProductResponses();
            var tmp = _productRepository.Entities.Where(p => p.CourseID == cId && p.Date.Date == dateTime.Date).OrderBy(p => p.TeeTime).ToList();
            if (tmp.Count <= 0)
            {
                return result;
            }
            foreach (var i in tmp)
            {
                ProductResponse productResponse = new ProductResponse();
                _mapper.Map(i, productResponse);
                if (i.LisExtensionID.Count() > 0)
                {
                    productResponse.LisExtension = _courseExtensionService.GetExtensionByListID(i.LisExtensionID);
                }
                result.Products.Add(productResponse);
                // result.TopProducts.Add(productResponse);
            }
            result.TopProducts = result.Products.Where(p => p.Promotion > 0).OrderBy(p => p.Date).ToList();
            return result;


        }
        /// <summary>
        /// sửa thông tin giờ chơi
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        async public Task<Product> Edit(Guid uId, Product product)
        {

            var result = _productRepository.Get((Guid)product.ID);
            if (result != null)
            {
                if (result.CreatedBy == uId)
                {
                    _productRepository.UpdateEntity(result);
                    return product;
                }
                {
                    throw new BadRequestException("Can not edit Product !");
                }
            }
            else
            {
                throw new BadRequestException("Can not edit Product !");
            }
        }

        /// <summary>
        /// Xóa giờ chơi
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<Product> Delete(Guid uId, Guid id)
        {
            Product result = _productRepository.Get(id);
            if (uId == result.CreatedBy)
            {
                _productRepository.RemoveEntity(result);
                return result;
            }
            else
            {
                throw new BadRequestException("Can not Delete this Product !");
            }
        }

        /// <summary>
        /// Xóa nhiều giờ chơi
        /// </summary>
        /// <param name="uId">Định danh người dùng</param>
        /// <param name="ids">danh sách định danh giờ chơi</param>
        /// <returns></returns>
         public bool Deletes(Guid uId, List<Guid> ids)
        {
            foreach (var i in ids)
            {
                Product result = _productRepository.Get(i);
                if (uId == result.CreatedBy)
                {
                    _productRepository.RemoveEntity(result);
                }
                else
                {
                    throw new BadRequestException("Can not Delete this Product !");
                }
            }
            return true;
        }
    }
}
