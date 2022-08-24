using AutoMapper;
using Golf.Core;
using Golf.Core.Dtos.Controllers.BookingController.Responses;
using Golf.Core.Exceptions;
using Golf.Domain.Bookings;
using Golf.Domain.Common;
using Golf.Domain.Courses;
using Golf.Domain.Shared;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.Courses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Locations
{
    public class LocationService
    {
        private readonly LocationRepository _locationRepository;
        private readonly CourseRepository _courseRepository;
        private readonly CourseService _courseService;
        private readonly GolferRepository _golferRepository;
        private readonly ProductRepository _productRepository;
        private readonly IMapper _mapper;

        public LocationService(
            GolferRepository golferRepository,
            ProductRepository productRepository,
            LocationRepository locationRepository,
            CourseRepository courseRepository,
            IMapper mapper,
            CourseService courseService)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _courseRepository = courseRepository;
            _courseService = courseService;
            _golferRepository = golferRepository;
            _productRepository = productRepository;
        }
        /// <summary>
        /// Lấy danh sách sân của location theo định danh location
        /// </summary>
        /// <param name="locationID">Định danh location</param>
        /// <returns></returns>
        async public Task<IEnumerable<Course>> GetCourseByLocationID(Guid locationID)
        {
            var location = _locationRepository.Get(locationID);
            var result = _courseRepository.Find(c => c.Location == location).ToList();
            return result;
        }
        /// <summary>
        /// Kiểm tra xem location có trong danh sách yêu thích không
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<bool> IsFavorite(Guid uId, Guid id)
        {
            var result = this.GetCourseByLocationID(id).Result.ToList();
            foreach (var i in result)
            {
                if (_courseService.IsFavorite(uId, i.ID).Result)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Lấy giờ chơi muộn nhất của location
        /// </summary>
        /// <param name="id">Định danh location</param>
        /// <param name="date">Ngày muốn lấy</param>
        /// <returns></returns>
        async public Task<TimeSpan> GetMaxLastTeeTimeFromNow(Guid id, DateTime date)
        {
            var tmp = this.GetCourseByLocationID(id).Result.ToList();
            List<TimeSpan> LastTeeTime = new List<TimeSpan>();
            foreach (var i in tmp)
            {
                LastTeeTime.Add(_courseService.GetMaxLasttTeeTimeFromNow(i.ID, date).Result);
            }
            if (LastTeeTime.Count() > 0)
            {
                return LastTeeTime.Max();
            }
            else
            {
                throw new BadRequestException("This course has't promotion ");
            }
        }
        /// <summary>
        /// Lấy giờ chơi sớm nhất của location
        /// </summary>
        /// <param name="id">Định danh location</param>
        /// <param name="date">Ngày muốn lấy</param>
        /// <returns></returns>
        async public Task<TimeSpan> GetMinFirstTeeTimeFromNow(Guid id, DateTime date)
        {
            var tmp = this.GetCourseByLocationID(id).Result.ToList();
            List<TimeSpan> FirstTeeTime = new List<TimeSpan>();
            foreach (var i in tmp)
            {
                FirstTeeTime.Add(_courseService.GetMinFirstTeeTimeFromNow(i.ID, date).Result);
            }
            if (FirstTeeTime.Count() > 0)
            {
                return FirstTeeTime.Max();
            }
            else
            {
                throw new BadRequestException("This course has't promotion ");
            }
        }
        /// <summary>
        /// Lấy điểm đánh giá trunng bình
        /// </summary>
        /// <param name="id">Định danh location</param>
        /// <returns></returns>
        async public Task<double> GetAVGCourseReviewPointByLocationID(Guid id)
        {
            var tmp = this.GetCourseByLocationID(id).Result.ToList().Select(c => c.ID);
            List<double> listPoint = new List<double>();
            if (tmp.Count() > 0)
            {
                foreach (var i in tmp)
                {
                    var t = _courseService.GetReviewPoint(i).Result;
                    if (t != null)
                    {
                        listPoint.Add(_courseService.GetReviewPoint(i).Result);
                    }
                }
                return listPoint.Average();
            }
            return 0;
        }

        /// <summary>
        /// tìm danh theo tên địa chỉ hoặc đất nước
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SearchBookingResponse SearchLocationByName(string searchKey,GPSAddress gPSAddress)
        {
            var reusult = new SearchBookingResponse();
            var tmp = _locationRepository.Find(l => l.Address.Trim().ToLower().Contains(searchKey) || l.Country.Trim().ToLower().Contains(searchKey));
            if (tmp.Count() == 0)
            {
                reusult.SearchLocationRespones = new List<SearchLocationRespone>();
            }
            foreach (var i in tmp)
            {
                reusult.SearchLocationRespones.Add(new SearchLocationRespone(i.ID, i.Name, i.Address, i.Country));
            }
            //var tmp1 = tmp.GroupBy(l => l.Country, l => l.Address, (c, a) => new { Adrees = a, Contry = c });
            //foreach (var i in tmp1)
            //{
            //    foreach (var j in i.Adrees.Distinct())
            //    {
            //        reusult.SearchLocationRespones.Add(new SearchLocationRespone(j, i.Contry));
            //    }
            //}
            var courseResult = _courseService.SearchByName(searchKey, 0, gPSAddress);
            foreach (var t in courseResult)
            {
                reusult.SearchCourseBookingRespones.Add(new SearchCourseBookingRespone(t.ID, t.Name, t.Address));
            }
            return reusult;
        }
        /// <summary>
        /// Lấy sản phẩm giảm giá nhiều nhất của Location
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<Product> GetMaxPromotionProductFromNow(Guid id, DateTime date)
        {
            var tmp = GetCourseByLocationID(id).Result.ToList();
            List<Product> listProduct = new List<Product>();
            foreach (var i in tmp)
            {
                var lp = _courseService.GetMaxPromotionProductFromNow(i.ID, date);
                if (lp.Result != null)
                {
                    listProduct.Add(lp.Result);
                }
            }
            if (listProduct.Count > 0)
            {
                var t = listProduct.Where(p => DateTime.Compare(p.Date, date) > 0).ToList();
                if (t.Count > 0)
                {
                    var max = t.Max(p => p.Promotion);
                    var result = listProduct.Where(p => DateTime.Compare(p.Date, date) > 0 && p.Promotion == max).ToList();
                    if (result.Count() > 0)
                    {
                        return result.FirstOrDefault();
                    }
                    else
                    {
                        throw new BadRequestException("This course has't promotion ");
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Lấy các location gợi ý Booing cho nguowif dung
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        async public Task<List<BookingResponse>> GetBookingLocation(Guid uId, int startIndex, GPSAddress gPSAddress)
        {
            var checkLocation = _productRepository.FindDetail(p => true).GroupBy(p => p.Course.Location);
            if (checkLocation == null)
                return new List<BookingResponse>();
            var validLocation = checkLocation.Select(t => t.Key.ID);
            List<BookingResponse> result = new List<BookingResponse>();
            var tmp = _locationRepository.Find(l => validLocation.Contains(l.ID)).Skip(startIndex).Take(Const.PageSize).ToList();
            foreach (var i in tmp)
            {
                if (this.GetCourseByLocationID(i.ID).Result.Count() > 0)
                {
                    BookingResponse t = new BookingResponse();
                    _mapper.Map(i, t);
                    t.LastTeeTime = this.GetMaxLastTeeTimeFromNow(i.ID, DateTime.Now).Result;
                    t.FirstTeeTime = this.GetMinFirstTeeTimeFromNow(i.ID, DateTime.Now).Result;
                    var product = this.GetMaxPromotionProductFromNow(i.ID, DateTime.Now).Result;
                    if (product != null)
                    {
                        t.Promotion = product.Promotion;
                        t.MembershipPromotion = product.MembershipPromotion;
                        t.Price = product.Price;
                    }
                    t.ReviewPoint = this.GetAVGCourseReviewPointByLocationID(i.ID).Result;
                    t.IsPromotion = (t.Promotion != 0) ? true : false;
                    t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
                    t.Distance = _courseService.GetDistance(gPSAddress, t.GPSAddress);
                    result.Add(t);
                }
            }
            return result;
        }
        /// <summary>
        /// Lấy thông tin booking của từng sần của location
        /// </summary>
        /// <param name="uId">Định danh người dùng hiện thời</param>
        /// <param name="locationID">Định danh location</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <param name="gPSAddress">Tọa độ GPs</param>
        /// <returns></returns>
        async public Task<List<BookingResponse>> GetBookingCourseByLocationId(Guid uId, Guid locationID, int startIndex, GPSAddress gPSAddress)
        {
            List<BookingResponse> result = new List<BookingResponse>();
            var location = _locationRepository.Get(locationID);
            if (location == null)
            {
                throw new NotFoundException("Not found location!");
            }
            var listCourses = _courseRepository.Find(c => c.Location == location).ToList().Skip(startIndex).Take(20);
            if (listCourses.Count() <= 0)
            {
                return new List<BookingResponse>();
            }
            foreach (var i in listCourses)
            {
                BookingResponse t = new BookingResponse();
                _mapper.Map(location, t);
                _mapper.Map(i, t);
                t.LastTeeTime = _courseService.GetMaxLasttTeeTimeFromNow(i.ID, DateTime.Now).Result;
                t.FirstTeeTime = _courseService.GetMinFirstTeeTimeFromNow(i.ID, DateTime.Now).Result;
                var product = _courseService.GetMaxPromotionProductFromNow(i.ID, DateTime.Now).Result;
                if (product != null)
                {
                    t.Promotion = product.Promotion;
                    t.MembershipPromotion = product.MembershipPromotion;
                    t.Price = product.Price;
                }
                t.ReviewPoint = _courseService.GetReviewPoint(i.ID).Result;
                t.IsPromotion = (t.Promotion != 0) ? true : false;
                t.IsFavorite = _courseService.IsFavorite(uId, i.ID).Result;
                t.Distance = _courseService.GetDistance(gPSAddress, t.GPSAddress);
                result.Add(t);
            }
            return result;
        }
    }
}
