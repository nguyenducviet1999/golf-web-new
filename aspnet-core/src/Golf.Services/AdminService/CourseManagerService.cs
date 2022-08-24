using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.Domain.Shared;
using Golf.EntityFrameworkCore;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Services.CourseAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.AdminService
{
    public class CourseManagerService
    {
        private readonly CourseAdminService _courseAdminService;
        private readonly CourseRepository _courseRepository;
        private readonly LocationRepository _locationRepository;
        private readonly DatabaseTransaction _databaseTransaction;

        public CourseManagerService(DatabaseTransaction databaseTransaction, LocationRepository locationRepository, CourseAdminService courseAdminService, CourseRepository courseRepository)
        {
            _locationRepository = locationRepository;
            _courseAdminService = courseAdminService;
            _courseRepository = courseRepository;
            _databaseTransaction = databaseTransaction;
        }

        /// <summary>
        /// Lấy danh sách yêu cầu tạo khu chơi golf
        /// </summary>
        /// <param name="startIndex">Vị trí phân trang</param>
        /// <returns>danh sách các địa điểm chơi golf</returns>
        public List<Location> GetLocationRequests(int startIndex)
        {
            var result = _locationRepository.Find(l => l.IsConfirmed == false).Skip(startIndex).Take(Const.PageSize).ToList();
            return result;
        }
        /// <summary>
        /// lấy danh sách các yêu cầu tạo sân chơi golf
        /// </summary>
        /// <param name="startIndex">vị trí phân trang</param>
        /// <returns></returns>
        public List<Course> GetCourseRequests(int startIndex)
        {
            var result = _courseRepository.Find(l => l.IsConfirmed == false).Skip(startIndex).Take(Const.PageSize).ToList();
            return result;
        }

        /// <summary>
        /// Xác nhận yêu cầu tạo địa điểm chơi golf
        /// </summary>
        /// <param name="locationID"></param>
        /// <returns></returns>
        public bool ConfirmLocationRequest(Guid locationID)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var location = _locationRepository.Get(locationID);
                if (location == null)
                    throw new NotFoundException("Location is not found");
                location.IsConfirmed = true;
                _locationRepository.SafeUpdate(location);
                var courses = _courseRepository.Find(c => c.LocationID == locationID);
                foreach (var c in courses)
                {
                    c.IsConfirmed = true;
                    _courseRepository.SafeUpdate(c);
                }
                _databaseTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
        /// <summary>
        /// Xác nhận yêu cầu tạo sân chơi golfk
        /// </summary>
        /// <param name="courseID"></param>
        /// <returns></returns>
        public async Task<bool> ConfirmCourseRequest(Guid courseID)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var course = _courseRepository.Get(courseID);
                if (course == null)
                    throw new NotFoundException("Course is not found");
                if (course.IsConfirmed == true)
                    throw new BadRequestException("Course is Confirmed");
                course.IsConfirmed = true;
                _courseRepository.SafeUpdate(course);
                var location = course.Location;
                location.IsConfirmed = true;
                _locationRepository.SafeUpdate(location);
                await _databaseTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _databaseTransaction.Rollback();
                throw new Exception(e.Message);
            }
        }
    }
}

