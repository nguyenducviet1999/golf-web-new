using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Resources;
using Golf.EntityFrameworkCore;
using Golf.Domain.Courses;


using Golf.Core.Exceptions;
using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Golf.Domain.GolferData;

namespace Golf.Services.CourseAdmin
{
    public class CourseAdminService
    {
        private readonly CourseRepository _courseRepository;
        private readonly LocationRepository _locationRepository;
        private readonly CourseExtensionRepository _courseExtensionRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly PhotoService _photoService;

        public CourseAdminService(
            CourseRepository courseRepository,
            LocationRepository locationRepository,
            CourseExtensionRepository courseExtensionRepository,
            DatabaseTransaction databaseTransaction,
            PhotoService photoService)
        {
            _courseRepository = courseRepository;
            _locationRepository = locationRepository;
            _courseExtensionRepository = courseExtensionRepository;
            _databaseTransaction = databaseTransaction;
            _photoService = photoService;
        }

        public Course GetCourseResponse(Course course)
        {
            return course;
        }

        public Course GetCourse(Guid CourseID)
        {
            return _courseRepository.Get(CourseID);
        }

        public Location GetLocation(Guid LocationID)
        {
            return _locationRepository.Get(LocationID);
        }

        public List<CourseExtension> GetCourseExtensions()
        {
            return _courseExtensionRepository.GetAll().ToList();
        }
        public Location AddLocation(Golfer currentGolfer, CreateLocationRequest request)
        {
            var ID = Guid.NewGuid();
            var location = new Location
            {
                ID = ID,
                OwnerID = currentGolfer.Id,
                Name = request.Name,
                Description = request.Description,
                PhoneNumber = request.PhoneNumber,
                FaxNumber = request.FaxNumber,
                Address = request.Address,
                GPSAddress = request.GPSAddress,
                Country = request.Country,
                Website = request.Website,
                HeadOffice = request.HeadOffice,
                Version = 0,
                IsConfirmed = false,
                MainVersionID = ID
            };
            _locationRepository.Add(location);
            return location;
        }

        public Location EditLocation(Golfer currentGolfer, Guid LocationID, EditLocationRequest request)
        {
            var currentLocation = _locationRepository.Get(LocationID);
            if (currentLocation != null && currentLocation.OwnerID == currentGolfer.Id)
            {
                var tempLocation = new Location
                {
                    ID = Guid.NewGuid(),
                    OwnerID = currentGolfer.Id,
                    Name = request.Name,
                    Description = request.Description,
                    PhoneNumber = request.PhoneNumber,
                    FaxNumber = request.FaxNumber,
                    Address = request.Address,
                    GPSAddress =request.GPSAddress,
                    Country = request.Country,
                    Website = request.Website,
                    HeadOffice = request.HeadOffice,
                    Version = -1,
                    IsConfirmed = false,
                    MainVersionID = currentLocation.ID,
                };
                _locationRepository.Add(tempLocation);
                return tempLocation;
            }
            throw new ForbiddenException("Access Denied");
        }
        /// <summary>
        /// G?i yêu c?u t?o m?t sân m?i
        /// </summary>
        /// <param name="currentGolfer">Ng??i dùng hi?n th?i</param>
        /// <param name="request">D? li?u sân</param>
        /// <returns></returns>
        public Course AddCourse(Golfer currentGolfer, CreateCourseRequest request)
        {
            Location location = _locationRepository.Get(request.LocationID);
            if (location != null)
            {
                var ID = Guid.NewGuid();
                var course = new Course
                {
                    ID = ID,
                    Location = location,
                    OwnerID = currentGolfer.Id,
                    Cover = "",
                    Name = request.Name,
                    Description = request.Description,
                    Extensions = request.Extensions,
                    MoreInformations = request.MoreInformations,
                    TotalHoles = request.TotalHoles,
                    Tees = request.Tees,
                    CourseHoles = request.CourseHoles,
                    PhotoNames = "",
                    Version = 0,
                    IsConfirmed = false,
                    MainVersionID = ID,
                    
                };
                int par = 0;
                foreach (var i in course.CourseHoles)
                {
                    par = par + i.Par;
                }
                course.Par = par;
                _courseRepository.Add(course);
                return course;
            }
            throw new NotFoundException("Can't find location");
        }

        /// <summary>
        /// S?a thông tin sân
        /// </summary>
        /// <param name="currentGolfer">??nh danh ng??i dùng hi?n th?i</param>
        /// <param name="CourseID">?inh danh sân</param>
        /// <param name="request">D? li?u ch?nh s?a</param>
        /// <returns></returns>
        public Course EditCourse(Golfer currentGolfer, Guid CourseID, EditCourseRequest request)
        {
            var currentCourse = _courseRepository.Get(CourseID);
            if (currentCourse != null && currentCourse.OwnerID == currentGolfer.Id)
            {
                Location location = _locationRepository.Get(request.LocationID);
                if (location != null)
                {
                    var tempCourse = new Course
                    {
                        ID = Guid.NewGuid(),
                        Location = location,
                        OwnerID = currentGolfer.Id,
                        Cover = "",
                        Name = request.Name,
                        Description = request.Description,
                        Extensions = request.Extensions,
                        MoreInformations = request.MoreInformations,
                        TotalHoles = request.TotalHoles,
                        Tees = request.Tees,
                        CourseHoles = request.CourseHoles,
                        PhotoNames = currentCourse.PhotoNames,
                        Version = 0,
                        IsConfirmed = false,
                        MainVersionID = currentCourse.ID
                    };
                    _courseRepository.Add(tempCourse);
                    return tempCourse;
                }
                throw new NotFoundException("Can't find location");
            }
            throw new ForbiddenException("Access Denied");
        }

        /// <summary>
        /// ??t ?nh bìa cho sân
        /// </summary>
        /// <param name="currentGolfer"></param>
        /// <param name="CourseID"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Course> SetCover(Golfer currentGolfer, Guid CourseID, SetCourseCoverRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var photo = await _photoService.SafeSavePhoto(CourseID, request.CourseCover, PhotoType.Course);
                var course = _courseRepository.Get(CourseID);
                if (course != null && course.OwnerID == currentGolfer.Id)
                {
                    course.Cover = $"{photo.Name}";
                    await _databaseTransaction.Commit();
                    return GetCourseResponse(course);
                }
                throw new ForbiddenException("Access Denied");
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Update course's photos error: {exception}");
            }
        }

        /// <summary>
        /// CH?nh s?a ?nh c?a sân
        /// </summary>
        /// <param name="currentGolfer">Ng??i dùng hi?n th?i</param>
        /// <param name="CourseID">??nh danh sân</param>
        /// <param name="request">D? li?u hình ?nh</param>
        /// <returns></returns>
        public async Task<Course> EditCoursePhotos(Golfer currentGolfer, Guid CourseID, EditCoursePhotosRequest request)
        {
            try
            {
                _databaseTransaction.BeginTransaction();
                var course = _courseRepository.Get(CourseID);
                if (course != null)
                {
                    if (course.OwnerID == currentGolfer.Id)
                    {
                        var photos = await _photoService.SavePhotos(CourseID, request.Files, PhotoType.Course);
                        var oldPhotoIDs = course.PhotoNames.Split(",").ToList();
                        course.PhotoNames = String.Join(",", photos.Select(photo => photo.Name));
                        _photoService.DeletePhotos(CourseID, oldPhotoIDs);
                        _courseRepository.SafeUpdate(course);
                        await _databaseTransaction.Commit();
                        return GetCourseResponse(course);
                    }
                    throw new ForbiddenException("Access denied");
                }
                throw new NotFoundException("Not found");
            }
            catch (Exception exception)
            {
                _databaseTransaction.Rollback();
                throw new Exception($"Update course's photos error: {exception}");
            }
        }
        /// <summary>
        /// L?y danh sách ??a ?i?m ch?i s? h?u
        /// </summary>
        /// <param name="currentGolfer">Ng??i dùng hi?n th?i</param>
        /// <returns></returns>
        public List<Location> GetOwnerLocations(Golfer currentGolfer)
        {
            var locations = _locationRepository.Find(
                location => location.OwnerID == currentGolfer.Id
                && location.MainVersionID == location.ID && location.IsConfirmed == true).ToList();
            return locations;
        }

        /// <summary>
        /// Laasyy danh sách sân s? h?u
        /// </summary>
        /// <param name="currentGolfer">Ng??i dùng hi?n th?i</param>
        /// <returns></returns>
        public List<Course> GetOwnerCourses(Golfer currentGolfer)
        {
            var courses = _courseRepository.Find(
                course => course.OwnerID == currentGolfer.Id
                && course.MainVersionID == course.ID && course.IsConfirmed == true).ToList();
            return courses;
        }

        /// <summary>
        /// L?y danh sách ??a ?i?m yêu c?u duy?t
        /// </summary>
        /// <param name="currentGolfer"></param>
        /// <returns></returns>
        public List<Location> GetOwnerPendingLocations(Golfer currentGolfer)
        {
            var locations = _locationRepository.Find(location => location.OwnerID == currentGolfer.Id && location.IsConfirmed == false);
            return locations.ToList();
        }

        /// <summary>
        /// L?y danh sách sân yêu c?u duy?t
        /// </summary>
        /// <param name="currentGolfer"></param>
        /// <returns></returns>
        public List<Course> GetOwnerPendingCourses(Golfer currentGolfer)
        {
            var courses = _courseRepository.Find(course => course.OwnerID == currentGolfer.Id && course.IsConfirmed == false);
            return courses.ToList();
        }

        /// <summary>
        /// Xóa sân
        /// </summary>
        /// <param name="currentGolfer"></param>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        public bool DeleteCourse(Golfer currentGolfer, Guid CourseID)
        {
            var course = _courseRepository.Get(CourseID);
            if (course != null && course.OwnerID == currentGolfer.Id)
            {
                _courseRepository.Remove(course);
                return true;
            }
            throw new ForbiddenException("Access Denied");
        }

        /// <summary>
        /// Xóa ??a ?i?m
        /// </summary>
        /// <param name="currentGolfer"></param>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public bool DeleteLocation(Golfer currentGolfer, Guid LocationID)
        {
            var location = _locationRepository.Get(LocationID);
            if (location != null && location.OwnerID == currentGolfer.Id)
            {
                _locationRepository.Remove(location);
                return true;
            }
            throw new ForbiddenException("Access Denied");
        }

    }
}