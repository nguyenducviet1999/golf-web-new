using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using Golf.EntityFrameworkCore.Repositories;

using Golf.Domain.Shared;

using Golf.Core.Common.Course;
using Golf.Core.Exceptions;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Controllers.CoursesController.Requests;
using Golf.Domain.Shared.Course;
using Golf.Services.Courses;
using Golf.Services.Products;
using Golf.Domain.Bookings;
using System.Threading.Tasks;
using Golf.Core.Resources;
using Golf.Core.Dtos.Controllers.BookingController.Responses;
using Golf.Core.Dtos.Controllers.CoursesController.Responses;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Common;
using Golf.Services.Locations;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Golf.Domain.Shared.Helper;
using Golf.EntityFrameworkCore;
using Golf.Domain.Resources;
using Golf.Domain.Shared.Resources;
using Golf.Services.Bookings;

namespace Golf.Services.Courses
{
    public class CourseService
    {
        private IHttpClientFactory _factory;
        readonly private CourseRepository _courseRepository;
        readonly private ScorecardRepository _scorecardRepository;
        readonly private ProductRepository _productRepository;
        readonly private CourseReviewService _courseReviewService;
        readonly private GolferRepository _golferRepository;
        readonly private ProductService _productService;
        readonly private CourseReviewRepository _courseReviewRepository;
        readonly private LocationRepository _locationRepository;
        readonly private CourseExtensionRepository _courseExtensionRepository;
        readonly private DatabaseTransaction _databaseTransaction;
        readonly private PhotoService _photoService;
        readonly private TransactionService _transactionService;
        SystemSettingRepository _systemSettingRepository;
        //readonly private LocationService _locationService;
        readonly private IMapper _mapper;

        public CourseService(TransactionService transactionService, PhotoService photoService, DatabaseTransaction databaseTransaction, SystemSettingRepository systemSettingRepository, ScorecardRepository scorecardRepository, IHttpClientFactory factory, CourseExtensionRepository courseExtensionRepository, LocationRepository locationRepository, CourseReviewRepository courseReviewRepository, ProductService productService, IMapper mapper, CourseRepository courseRepository, ProductRepository productRepository, CourseReviewService courseReviewService, GolferRepository golferRepository)
        {
            _transactionService = transactionService;
            _photoService = photoService;
            _databaseTransaction = databaseTransaction;
            _systemSettingRepository = systemSettingRepository;
            _scorecardRepository = scorecardRepository;
            _factory = factory;
            _courseRepository = courseRepository;
            _productRepository = productRepository;
            _courseReviewService = courseReviewService;
            _golferRepository = golferRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
            _productService = productService;
            _courseExtensionRepository = courseExtensionRepository;
            _courseReviewRepository = courseReviewRepository;
            _scorecardRepository = scorecardRepository;
            //_locationService = locationService;
        }
        /// <summary>
        /// Lấy dư liệu sân từ định danh
        /// </summary>
        /// <param name="uId">Định danh người dùng</param>
        /// <param name="id">Định danh sân</param>
        /// <returns></returns>
        async public Task<Course> Get(Guid uId, Guid id)
        {
            var result = _courseRepository.Get(id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found couese !");
            }
        }
        /// <summary>
        /// Lọc danh sách sân theo tiêu chí lọc
        /// </summary>
        /// <param name="uId">Id người dùng</param>
        /// <param name="startIndex">số thứ tự phần tử đầu tiên</param>
        /// <param name="filter">Bộ lọc</param>
        /// <param name="date">thời gian chỉ định</param>
        /// <param name="gPSAddress">định vị vị trí</param>
        /// <returns></returns>
        async public Task<IEnumerable<BookingResponse>> GetBookingCourseByFilter(Guid uId, int startIndex, int filter, DateTime date, GPSAddress gPSAddress)
        {
            switch (filter)
            {
                //Lọc các sân yêu thích
                case CourseFilter.Favorite:
                    {
                        List<BookingResponse> result = new List<BookingResponse>();
                        var curGolfer = _golferRepository.Get(uId);
                        var tmp = _courseRepository.Find(c => curGolfer.CourseFavorites.Contains(c.ID)).ToList();
                        var products = _productRepository.Find(p => curGolfer.CourseFavorites.Contains(p.CourseID) && p.Date.Date == date.Date && p.FullBooking == false).ToList();
                        List<Guid> listCoursesId = new List<Guid>();
                        if (products.Count() <= 0)
                        {
                            return null;
                        }
                        foreach (var i in products)
                        {
                            if (listCoursesId.Find(ci => ci == i.CourseID) == Guid.Empty)
                            {
                                listCoursesId.Add(i.CourseID);
                            }
                        }
                        var listCourses = _courseRepository.Find(c => listCoursesId.Contains(c.ID)).ToList().Skip(startIndex).Take(Const.PageSize);
                        foreach (var i in listCourses)
                        {
                            var location = i.Location;
                            BookingResponse t = new BookingResponse();
                            _mapper.Map(location, t);
                            _mapper.Map(i, t);
                            t.LastTeeTime = this.GetMaxLasttTeeTimeInDate(i.ID, date).Result;
                            t.FirstTeeTime = this.GetMinFirstTeeTimeInDate(i.ID, date).Result;
                            var product = this.GetMaxPromotionProductInDate(i.ID, date).Result;
                            if (product != null)
                            {
                                t.Promotion = product.Promotion;
                                t.MembershipPromotion = product.MembershipPromotion;
                                t.Price = product.Price;
                            }
                            t.ReviewPoint = this.GetReviewPoint(i.ID).Result;
                            t.IsPromotion = (t.Promotion != 0) ? true : false;
                            t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
                            t.TotalReview = this.GetTotalReview(i.ID);
                            t.Distance = GetDistance(gPSAddress, t.GPSAddress);
                            result.Add(t);
                        }
                        return result;
                    }
                //lọc các sân xung quanh
                case CourseFilter.Nearest:
                    {
                        List<BookingResponse> result = new List<BookingResponse>();
                        //var curGolfer = _golferRepository.Get(uId);
                        List<CourseDistance> courseDistances = new List<CourseDistance>();
                        if (gPSAddress == null || gPSAddress.Validate() == false)
                        {
                            return new List<BookingResponse>();
                        }
                        var systemSetting = _systemSettingRepository.FirstOrDefault();
                        var maxDistance = 0;
                        if (systemSetting == null)
                        {
                            maxDistance = 50;
                        }
                        else
                        {
                            maxDistance = systemSetting.Setting.MaxLengthOfCourseNearest;
                        }
                        var longitudeUnitLength = DistanceHelper.GetLongitudeUnitLength((double)gPSAddress.Longitude);
                        var tmp = _courseRepository.FindDetail(c =>
                                            (((double)c.Location.GPSAddress.Latitude - (double)gPSAddress.Latitude) * longitudeUnitLength < maxDistance && (c.Location.GPSAddress.Latitude - (double)gPSAddress.Latitude) * longitudeUnitLength > (maxDistance * (-1)))
                                            && (((double)c.Location.GPSAddress.Longitude - (double)gPSAddress.Longitude) * Const.LengthOfLongitude < maxDistance && ((double)c.Location.GPSAddress.Longitude - (double)gPSAddress.Longitude) * Const.LengthOfLongitude > (maxDistance * (-1)))
                                            );
                        foreach (var i in tmp)
                        {
                            CourseDistance t = new CourseDistance();
                            var distance = this.GetDistance(i.Location.GPSAddress, gPSAddress);
                            if (distance == "")
                            {
                                break;
                            }
                            t.ID = i.ID;
                            t.Distance = int.Parse(distance);

                            if (t.Distance <= maxDistance)
                            {
                                courseDistances.Add(t);
                            }
                        }
                        var nearestCourseIDs = courseDistances.OrderBy(c => c.Distance).Take(Const.PageSize).Select(c => c.ID).ToList();
                        var products = _productRepository.Find(p => nearestCourseIDs.Contains(p.CourseID) && p.Date.Date == date.Date && p.FullBooking == false).ToList();
                        List<Guid> listCoursesId = new List<Guid>();
                        if (products.Count() <= 0)
                        {
                            return null;
                        }
                        foreach (var i in products)
                        {
                            if (listCoursesId.Find(ci => ci == i.CourseID) == Guid.Empty)
                            {
                                listCoursesId.Add(i.CourseID);
                            }
                        }
                        var listCourses = _courseRepository.Find(c => listCoursesId.Contains(c.ID)).ToList().Skip(startIndex).Take(Const.PageSize);
                        foreach (var i in listCourses)
                        {
                            var location = i.Location;
                            BookingResponse t = new BookingResponse();
                            _mapper.Map(location, t);
                            _mapper.Map(i, t);
                            t.LastTeeTime = this.GetMaxLasttTeeTimeInDate(i.ID, date).Result;
                            t.FirstTeeTime = this.GetMinFirstTeeTimeInDate(i.ID, date).Result;
                            var product = this.GetMaxPromotionProductInDate(i.ID, date).Result;
                            if (product != null)
                            {
                                t.Promotion = product.Promotion;
                                t.MembershipPromotion = product.MembershipPromotion;
                                t.Price = product.Price;
                            }
                            t.ReviewPoint = this.GetReviewPoint(i.ID).Result;
                            t.IsPromotion = (t.Promotion != 0) ? true : false;
                            t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
                            t.TotalReview = this.GetTotalReview(i.ID);
                            t.Distance = GetDistance(gPSAddress, t.GPSAddress);
                            result.Add(t);
                        }
                        return result;
                    }
                //Lấy các sân có khuyến mại
                case CourseFilter.Promotion:
                    {
                        List<BookingResponse> result = new List<BookingResponse>();
                        var products = _productRepository.Find(p => p.Promotion > 0 && p.Date.Date == date.Date && p.FullBooking == false).ToList();
                        List<Guid> listCoursesId = new List<Guid>();
                        if (products.Count() <= 0)
                        {
                            return null;
                        }
                        foreach (var i in products)
                        {
                            if (listCoursesId.Find(ci => ci == i.CourseID) == Guid.Empty)
                            {
                                listCoursesId.Add(i.CourseID);
                            }
                        }
                        var listCourses = _courseRepository.Find(c => listCoursesId.Contains(c.ID)).ToList().Skip(startIndex).Take(Const.PageSize);
                        foreach (var i in listCourses)
                        {
                            var location = i.Location;
                            BookingResponse t = new BookingResponse();
                            _mapper.Map(location, t);
                            _mapper.Map(i, t);
                            t.LastTeeTime = this.GetMaxLasttTeeTimeInDate(i.ID, date).Result;
                            t.FirstTeeTime = this.GetMinFirstTeeTimeInDate(i.ID, date).Result;
                            var product = this.GetMaxPromotionProductInDate(i.ID, date).Result;
                            if (product != null)
                            {
                                t.Promotion = product.Promotion;
                                t.MembershipPromotion = product.MembershipPromotion;
                                t.Price = product.Price;
                            }
                            t.ReviewPoint = this.GetReviewPoint(i.ID).Result;
                            t.IsPromotion = (t.Promotion != 0) ? true : false;
                            t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
                            t.TotalReview = this.GetTotalReview(i.ID);
                            t.Distance = GetDistance(gPSAddress, t.GPSAddress);
                            result.Add(t);
                        }
                        return result;
                    }
                // Lấy danh danh sân cuối tuần có giò chơi cho thuê
                case CourseFilter.Weekend:
                    {
                        List<BookingResponse> result = new List<BookingResponse>();
                        var products = _productRepository.Find(p => p.Date.Date == date.Date && p.FullBooking == false).ToList();
                        List<Guid> listCoursesId = new List<Guid>();
                        if (products.Count() <= 0)
                        {
                            return null;
                        }
                        foreach (var i in products)
                        {
                            if (listCoursesId.Find(ci => ci == i.CourseID) == Guid.Empty)
                            {
                                listCoursesId.Add(i.CourseID);
                            }
                        }
                        var listCourses = _courseRepository.Find(c => listCoursesId.Contains(c.ID)).ToList().Skip(startIndex).Take(Const.PageSize);
                        foreach (var i in listCourses)
                        {
                            var location = i.Location;
                            BookingResponse t = new BookingResponse();
                            _mapper.Map(location, t);
                            _mapper.Map(i, t);

                            t.LastTeeTime = this.GetMaxLasttTeeTimeInDate(i.ID, date).Result;
                            t.FirstTeeTime = this.GetMinFirstTeeTimeInDate(i.ID, date).Result;
                            var product = this.GetMaxPromotionProductInDate(i.ID, date).Result;
                            if (product != null)
                            {
                                t.Promotion = product.Promotion;
                                t.MembershipPromotion = product.MembershipPromotion;
                                t.Price = product.Price;
                            }

                            t.ReviewPoint = this.GetReviewPoint(i.ID).Result;
                            t.IsPromotion = (t.Promotion != 0) ? true : false;
                            t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
                            t.TotalReview = this.GetTotalReview(i.ID);
                            t.Distance = GetDistance(gPSAddress, t.GPSAddress);
                            result.Add(t);
                        }
                        return result;
                    }
                default:
                    {
                        throw new BadRequestException("Fillter is not value !");
                    }
            }
            return new List<BookingResponse>();
        }

        /// <summary>
        /// Lấy điểm đánh giá sân
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<double> GetReviewPoint(Guid id)
        {
            var result = _courseReviewService.GetListCourseReviewPointByCourseId(id).Result;
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found couese !");
            }
        }

        /// <summary>
        /// Lấy dữ liệu sân cho màn hình đặt sân
        /// </summary>
        /// <param name="uId">id người dùng hiện tại</param>
        /// <param name="courseID">Định danh sân</param>
        /// <param name="date">Thời điểm chỉ định</param>
        /// <param name="gPSAddress">Vị trí người dùng</param>
        /// <returns></returns>
        async public Task<BookingResponse> GetCourseBookingByID(Guid uId, Guid courseID, DateTime date, GPSAddress gPSAddress)
        {
            var i = _courseRepository.Get(courseID);
            if (i == null)
            {
                throw new NotFoundException("Not Found Course!");
            }
            var location = i.Location;
            BookingResponse t = new BookingResponse();
            _mapper.Map(location, t);
            _mapper.Map(i, t);

            t.LastTeeTime = this.GetMaxLasttTeeTimeFromNow(i.ID, date).Result;
            t.FirstTeeTime = this.GetMinFirstTeeTimeFromNow(i.ID, date).Result;
            var product = this.GetMaxPromotionProductFromNow(i.ID, date).Result;
            if (product != null)
            {
                t.Promotion = product.Promotion;
                t.MembershipPromotion = product.MembershipPromotion;
                t.Price = product.Price;
            }
            t.TotalReview = this.GetTotalReview(courseID);
            t.ReviewPoint = this.GetReviewPoint(i.ID).Result;
            t.IsPromotion = (t.Promotion != 0) ? true : false;
            t.IsFavorite = this.IsFavorite(uId, i.ID).Result;
            t.Distance = GetDistance(gPSAddress, t.GPSAddress);
            return t;
        }

        /// <summary>
        /// lấy tổng số lượt đánh giá sân
        /// </summary>
        /// <param name="id">Id sân</param>
        /// <returns></returns>
        public int GetTotalReview(Guid id)
        {
            var result = _courseReviewRepository.Find(cr => cr.CourseID == id).Count();
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found couese !");
            }
        }

        /// <summary>
        /// kiểm tra sân có phải sân yêu thích hay không
        /// </summary>
        /// <param name="uId">ID người dùng</param>
        /// <param name="id">id sân</param>
        /// <returns></returns>
        async public Task<bool> IsFavorite(Guid uId, Guid id)
        {
            var result = _golferRepository.Get(uId);
            if (result.CourseFavorites != null)
            {
                if (result.CourseFavorites.Find(ci => ci == id) != Guid.Empty)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// kiểm tra sân có khuyến mại hay không trong thời gian chỉ định
        /// </summary>
        /// <param name="courseID">ĐỊnh danh sân</param>
        /// <param name="dateTime">THời gian chỉ định</param>
        /// <returns></returns>
        public bool IsPromotion(Guid courseID, DateTime dateTime)
        {
            var result = _productRepository.Find(p => p.CourseID == courseID && (p.Promotion != null || p.Promotion != 0) && p.Date.Date == dateTime.Date && p.FullBooking == false).ToList();
            if (result.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Lấy các product có khuyến mại trong ngày 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        async public Task<Product> GetMaxPromotionProductInDate(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0 && p.FullBooking == false).ToList().Max(p => p.Promotion);
            var result = _productRepository.Find(p => p.CourseID == id && p.Date.Date == date.Date && p.Promotion == tmp).ToList();
            if (result.Count() > 0)
            {
                return result.FirstOrDefault();
            }
            else
            {
                throw new BadRequestException("This course has't promotion ");
            }
        }
        async public Task<Product> GetMinPromotionProductInDate(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0 && p.FullBooking == false).ToList().Min(p => p.Promotion);
            var result = _productRepository.Find(p => p.CourseID == id && p.Date == date && p.Promotion == tmp).ToList();
            if (result.Count() > 0)
            {
                return result.FirstOrDefault();
            }
            else
            {
                throw new BadRequestException("This course has't promotion ");
            }
        }

        /// <summary>
        /// Lấy các product có khuyến mại lớn nhất tính từ thời gian nhập vào
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        async public Task<Product> GetMaxPromotionProductFromNow(Guid id, DateTime date)
        {
            var t = _productRepository.Entities.Where(p => p.CourseID == id && ((DateTime.Compare(p.Date.Date, date.Date) >= 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0) || DateTime.Compare(p.Date.Date, date.Date) > 0)).ToList();
            if (t.Count() > 0)
            {
                var tmp = t.Max(p => p.Promotion);
                //var result = _productRepository.Find(p => p.CourseID == id && DateTime.Compare(p.Date.Date, date.Date) >= 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0 && p.Promotion == tmp).ToList();
                var result = t.Find(p => p.Promotion == tmp);
                return result;
            }
            else return null;
        }

        /// <summary>
        /// Lấy thời điểm Tee time đầu tiên của sân 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        async public Task<TimeSpan> GetMinFirstTeeTimeFromNow(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && ((DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0) || DateTime.Compare(p.Date.Date, date.Date) > 0) && p.FullBooking == false).ToList();
            if (tmp.Count() > 0)
            {
                var result = tmp.Min(p => p.TeeTime);
                return result;
            }
            return new TimeSpan();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        async public Task<TimeSpan> GetMaxLasttTeeTimeFromNow(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && ((DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0) || DateTime.Compare(p.Date.Date, date.Date) > 0)).ToList();
            if (tmp.Count() > 0)
            {
                var result = tmp.Max(p => p.TeeTime);
                return result;
            }
            return new TimeSpan();
        }

        async public Task<TimeSpan> GetMinFirstTeeTimeInDate(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0 && p.FullBooking == false).ToList();
            if (tmp.Count() > 0)
            {
                var result = tmp.Min(p => p.TeeTime);
                return result;
            }

            return new TimeSpan();
        }

        async public Task<TimeSpan> GetMaxLasttTeeTimeInDate(Guid id, DateTime date)
        {
            var tmp = _productRepository.Entities.Where(p => p.CourseID == id && DateTime.Compare(p.Date.Date, date.Date) == 0 && TimeSpan.Compare(p.TeeTime, date.TimeOfDay) > 0 && p.FullBooking == false).ToList();
            if (tmp.Count() > 0)
            {
                var result = tmp.Max(p => p.TeeTime);
                return result;
            }
            return new TimeSpan();
        }

        public CourseResponse GetCourseResponse(Course Course, GPSAddress gPSAddress)
        {
            var reusult = new CourseResponse();
            _mapper.Map(Course, reusult);
            reusult.Address = Course.Location.Address;
            if (gPSAddress != null)
            {
                reusult.Distance = GetDistance(gPSAddress, Course.Location.GPSAddress);
            }
            /// Get Point
            /// Get Location
            /// Get Extension
            return reusult;
        }

        /// <summary>
        /// Lấy dữ liệu thông ti hố của sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="color">Loại hình chơi</param>
        /// <returns></returns>
        public List<CourseHoleResponse> GetCourseHolesByTee(Guid CourseID, TeeColor color)
        {
            var course = _courseRepository.Get(CourseID);
            if (course != null)
            {
                List<CourseHoleResponse> courseHoles = new List<CourseHoleResponse>();
                foreach (CourseHole courseHole in course.CourseHoles)
                {
                    int distance = 0;
                    switch (color)
                    {
                        case TeeColor.xFF000000:
                            {
                                distance = courseHole.BlackTeeDistance;
                                break;
                            }
                        case TeeColor.xFF2385F8:
                            {
                                distance = courseHole.BlueTeeDistance;
                                break;
                            }
                        case TeeColor.xFFFB2B2B:
                            {
                                distance = courseHole.RedTeeDistance;
                                break;
                            }
                        case TeeColor.xFFFFFFFF:
                            {
                                distance = courseHole.WhiteTeeDistance;
                                break;
                            }
                        default:
                            throw new BadRequestException("Cant not find tee");
                    }
                    courseHoles.Add(new CourseHoleResponse
                    {
                        Index = courseHole.Index,
                        StrokeIndex = courseHole.StrokeIndex,
                        Par = courseHole.Par,
                        YardDistance = distance,
                        MeterDistance = distance,
                    });
                }
                //        List<CourseHoleResponse> courseHoles = new List<CourseHoleResponse>();
                return courseHoles;
                //    throw new NotFoundException("Not found");
            }
            return new List<CourseHoleResponse>();
        }
        //Course service
        public List<CourseResponse> GetCourses(int startIndex, GPSAddress gPSAddress)
        {
            var courses = _courseRepository.GetAllDetail().Skip(startIndex).Take(Const.PageSize).ToList();
            List<CourseResponse> coursesResponse = new List<CourseResponse>();
            foreach (Course course in courses)
            {
                coursesResponse.Add(GetCourseResponse(course, gPSAddress));
            }
            return coursesResponse;
        }

        public CourseResponse GetCourse(Guid CourseID, GPSAddress gPSAddress)
        {
            var course = _courseRepository.Get(CourseID);
            if (course != null)
            {
                return GetCourseResponse(course, gPSAddress);
            }
            else
            {
                throw new NotFoundException("Not found");
            }
        }
        public CourseDetailResponse GetCourseDetail(Guid golferID, Guid CourseID)
        {
            var course = _courseRepository.Get(CourseID);
            if (course == null)
            {
                throw new NotFoundException("Not found");
            }
            var location = course.Location;
            CourseDetailResponse t = new CourseDetailResponse();
            _mapper.Map(location, t);
            _mapper.Map(course, t);
            t.MoreInformation = course.MoreInformations;
            t.IsFavorite = this.IsFavorite(golferID, CourseID).Result;
            t.ReviewPoint = this.GetReviewPoint(CourseID).Result;
            t.CourseReviewResponse = this.GetCourseReviews(CourseID,0,1);
            t.CourseExtensions = _courseExtensionRepository.Find(cx => course.Extensions.Contains(cx.ID)).ToList();

            // t.Distance = _locationService.GetDistance(gPSAddress, t.GPSAddress);
            return t;
        }

        public List<Tee> GetCourseTees(Guid CourseID)
        {
            var course = _courseRepository.Get(CourseID);
            if (course != null)
            {
                return course.Tees.ToList();
            }
            throw new NotFoundException("Not found");
        }
        //CourseReviewService
        public double GetCourseReviewPoint(Guid CourseID)
        {
            var courseReviews = _courseReviewRepository.Find(courseReview => courseReview.CourseID == CourseID).ToList();
            if (courseReviews.Count() > 0)
            {
                var result = courseReviews.Average(courseReview => courseReview.Point);
                return Math.Round(result, 1);
            }
            return 0;
        }

        /// <summary>
        /// Thêm đánh giá sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="currentGolfer">Người đánh giá</param>
        /// <param name="request">Dữ liệu đánh giá</param>
        public async Task<CourseReviewResponse> AddCourseReview(Guid CourseID, Golfer currentGolfer, AddCourseReviewRequest request)
        {
            var result = _transactionService.GetTransactionCompleted(currentGolfer.Id, 0);
            if (result.Count() <= 0)
            {
                throw new ForbiddenException("Access denied!");
            }
            List<Photo> photos = new List<Photo>();
            _databaseTransaction.BeginTransaction();
            try
            {

                if (request.Photos != null)
                {
                    foreach (var i in request.Photos)
                    {
                        Photo photo = new Photo();
                        photo = _photoService.SafeSavePhoto(currentGolfer.Id, i, PhotoType.CourseReview).Result;
                        photos.Add(photo);
                    }

                }
                if (_courseRepository.Get(CourseID) == null)
                    throw new NotFoundException("Not found");
                CourseReview courseReview = new CourseReview();
                courseReview.OwnerID = currentGolfer.Id;
                courseReview.Point = request.Point;
                courseReview.CourseID = CourseID;
                courseReview.Content = request.Content;
                if (photos.Count() > 0)
                {
                    courseReview.SetPhotoNames(photos.Select(p => p.Name).ToList());
                }
                _courseReviewRepository.SafeAdd(courseReview);
                await _databaseTransaction.Commit();
                return this.GetCourseReview(courseReview.ID);
            }
            catch (Exception e)
            {
                foreach (var i in photos)
                {
                    _photoService.DeletePhoto(i.Name);
                }
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Sửa đánh giá sân
        /// </summary>
        /// <param name="CourseID">định danh sân</param>
        /// <param name="currentGolfer">Người dùng hiện thời</param>
        /// <param name="request">Dư liệu chỉnh sửa</param>
        public async Task<CourseReviewResponse> EditCourseReview(Guid CourseID, Golfer currentGolfer, EditCourseReviewRequest request)
        {
            var currentCourseReview = _courseReviewRepository.Get(request.CourseReviewID);
            if (currentCourseReview == null)
            {
                throw new NotFoundException("Not found");
            }
            if (currentCourseReview.CreatedBy != currentGolfer.Id)
            {
                {
                    throw new ForbiddenException("Access denied");
                }
            }

            List<Photo> photos = new List<Photo>();
            try
            {
                if (request.Photos != null)
                {
                    foreach (var i in request.Photos)
                    {
                        var photo = await _photoService.SavePhoto(currentGolfer.Id, i, PhotoType.CourseReview);
                        photos.Add(photo);
                    }

                }

                if (_courseRepository.Get(CourseID) == null)
                    throw new NotFoundException("Not found");

                var courseReview = new CourseReview
                {
                    CourseID = CourseID,
                    CreatedBy = currentGolfer.Id,
                    Content = request.Content,
                    Point = request.Point,
                };
                if (photos.Count() > 0)
                {
                    //StringHelper.SetGuids(courseReview.PhotoNames, photos.Select(p => p.ID).ToList());
                }
                _courseReviewRepository.SafeAdd(courseReview);
                await _databaseTransaction.Commit();
                return this.GetCourseReview(courseReview.ID);
            }
            catch (Exception e)
            {
                foreach (var i in photos)
                {
                    _photoService.DeletePhoto(i.Name);
                }
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }

        }

        /// <summary>
        /// Xóa dánh giá sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <param name="currentGolfer">Người dùng hiện thời</param>
        /// <param name="request">Định danh đánh giá</param>
        public void DeleteCourseReview(Guid CourseID, Golfer currentGolfer, DeleteCourseReviewRequest request)
        {
            var currentCourseReview = _courseReviewRepository.Get(request.CourseReviewID);
            if (currentCourseReview != null)
            {
                if (currentCourseReview.CreatedBy == currentGolfer.Id)
                {
                    _courseReviewRepository.Remove(currentCourseReview);
                }
                else
                {
                    throw new ForbiddenException("Access denied");
                }
            }
            else
            {
                throw new NotFoundException("Not found");
            }
        }

        /// <summary>
        /// Lấy danh sách đính giá sân
        /// </summary>
        /// <param name="CourseID">Định danh sân</param>
        /// <returns></returns>
        public List<CourseReviewResponse> GetCourseReviews(Guid CourseID,int startIndex, int pageSize)
        {
            var courseReviews = _courseReviewRepository.Find(courseReview => courseReview.CourseID == CourseID).Skip(startIndex).Take(pageSize).ToList();
            List<CourseReviewResponse> courseReviewsResponse = new List<CourseReviewResponse>();
            foreach (CourseReview courseReview in courseReviews)
            {
                courseReviewsResponse.Add(GetCourseReview(courseReview));
            }
            return courseReviewsResponse;
        }
        /// <summary>
        /// Lấy đánh giá sân
        /// </summary>
        /// <param name="courseReview"></param>
        /// <returns></returns>
        public CourseReviewResponse GetCourseReview(CourseReview courseReview)
        {
            return new CourseReviewResponse()
            {
                Content = courseReview.Content,
                Point = courseReview.Point,
                PhotoNames = StringHelper.GetListStrings(courseReview.PhotoNames),
                Golfer = _mapper.Map<Golfer, MinimizedGolfer>(courseReview.Owner)
            };
        }
        public CourseReviewResponse GetCourseReview(Guid courseReviewID)
        {
            var courseReview = _courseReviewRepository.Find(cr => cr.ID == courseReviewID).FirstOrDefault();
            if (courseReview == null)
                throw new NotFoundException("Not found course review!");
            return new CourseReviewResponse()
            {
                Content = courseReview.Content,
                Point = courseReview.Point,
                PhotoNames = StringHelper.GetListStrings(courseReview.PhotoNames),
                Golfer = _mapper.Map<Golfer, MinimizedGolfer>(courseReview.Owner)
            };
        }

        /// <summary>
        /// Tìm kiếm sân theo tên
        /// </summary>
        /// <param name="courseName">Từ khóa tìm kiếm</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <param name="gPSAddress">Tọa độ địa lý(Nếu có))</param>
        /// <returns></returns>
        public List<CourseResponse> SearchByName(string courseName, int startIndex, GPSAddress gPSAddress)
        {
            var courses = _courseRepository.Find(c => c.Name.Trim().ToLower().Contains(courseName.Trim().ToLower())).Skip(startIndex).Take(Const.PageSize).ToList();
            List<CourseResponse> coursesResponse = new List<CourseResponse>();
            if (courses.Count() > 0)
            {

                foreach (Course course in courses)
                {

                    CourseResponse tmp = new CourseResponse();
                    _mapper.Map(course.Location, tmp);
                    _mapper.Map(course, tmp);
                    tmp.Distance = GetDistance(gPSAddress, course.Location.GPSAddress);

                    coursesResponse.Add(tmp);
                }

            }
            return coursesResponse;
        }

        /// <summary>
        /// Lấy danh sách sân đã chơi
        /// </summary>
        /// <param name="currentID">Định danh người dùng hiện thời</param>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <param name="gPSAddress">Tọa độ vị trí</param>
        /// <returns></returns>
        public List<CourseResponse> GetCoursePlayed(Guid currentID, int startIndex, GPSAddress gPSAddress)
        {
            var scorecards = _scorecardRepository.Find(sc => sc.OwnerID == currentID).GroupBy(sc => sc.CourseID, sc => sc.Date);
            if (scorecards.Count() == 0)
            {
                return new List<CourseResponse>();
            }
            var tmp = scorecards.Select(sc => new { CourseID = sc.Key, Time = sc.ToList().Max() });
            var courseIDs = tmp.OrderByDescending(sc => sc.Time).Select(sc => sc.CourseID);
            var courses = _courseRepository.Find(c => courseIDs.Contains(c.ID)).Skip(startIndex).Take(Const.PageSize).ToList();
            List<CourseResponse> coursesResponses = new List<CourseResponse>();
            if (courses.Count() > 0)
            {
                foreach (Course course in courses)
                {
                    CourseResponse t = new CourseResponse();
                    _mapper.Map(course.Location, t);
                    _mapper.Map(course, t);
                    t.Distance = GetDistance(gPSAddress, course.Location.GPSAddress);
                    coursesResponses.Add(t);
                }
            }
            return coursesResponses;
        }

        /// <summary>
        /// Lấy danh sách  sân gần đây
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <param name="gPSAddress">Tọa độ vị trí</param>
        /// <returns></returns>
        public List<CourseResponse> GetCourseNearest(int startIndex, GPSAddress gPSAddress)
        {
            var tmp = _courseRepository.GetAllDetail();
            if (tmp.Count() == 0)
                return new List<CourseResponse>();
            List<CourseDistance> courseDistances = new List<CourseDistance>();
            foreach (var i in tmp)
            {
                CourseDistance t = new CourseDistance();
                t.ID = i.ID;
                t.Distance = int.Parse(this.GetDistance(i.Location.GPSAddress, gPSAddress));
                courseDistances.Add(t);
            }
            var nearestCourseIDs = courseDistances.OrderBy(c => c.Distance).Take(Const.PageSize).Select(c => c.ID).ToList();
            var courses = _courseRepository.Find(c => nearestCourseIDs.Contains(c.ID)).Skip(startIndex).Take(Const.PageSize).ToList();
            List<CourseResponse> coursesResponses = new List<CourseResponse>();
            if (courses.Count() > 0)
            {
                foreach (Course course in courses)
                {
                    CourseResponse t = new CourseResponse();
                    _mapper.Map(course.Location, t);
                    _mapper.Map(course, t);
                    t.Distance = GetDistance(gPSAddress, course.Location.GPSAddress);
                    coursesResponses.Add(t);
                }
            }
            return coursesResponses;
        }

        /// <summary>
        /// Sử dụng API VietMap để tính khoảng cách giữa 2 tọa độ địa lý
        /// </summary>
        /// <param name="sourceGPSAsdress"></param>
        /// <param name="destGPSAsdress"></param>
        /// <returns></returns>
        public string GetDistance(GPSAddress sourceGPSAsdress, GPSAddress destGPSAsdress)//sourceGPSAsdress="21.035098,105.850309";destGPSAsdress="21.035793,105.833526"
        {
            try
            {
                HttpClient client = _factory.CreateClient();
                if (sourceGPSAsdress ==null|| destGPSAsdress ==null|| sourceGPSAsdress.Validate() == false || destGPSAsdress.Validate() == false)
                    return "";
                client.BaseAddress = new Uri("https://maps.vietmap.vn");
                var response = client.GetAsync("/api/route?api-version=1.1&apikey=383a90729d0590f9e1074083a11791ff64767fb56c1d9c4f&vehicle=car&point=" + sourceGPSAsdress.toString() + "&point=" + destGPSAsdress.toString()).Result;
                string jsonData = response.Content.ReadAsStringAsync().Result;
                string data = JObject.Parse(jsonData)["paths"][0]["distance"].ToString();         //var tmp1 = GetJArrayValue(tmp, "paths");
                return Math.Round((double.Parse(data) / 1000), 0).ToString();
            }
            catch
            {
                throw new NotFoundException("Can't calculate distance!");
            }
        }
    }
}