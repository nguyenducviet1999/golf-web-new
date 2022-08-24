using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Linq;

using Golf.Domain.Resources;
using Golf.Domain.Shared.Resources;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared;
using Golf.Core.Exceptions;

namespace Golf.Services
{
    public class PhotoService
    {
        private readonly PhotoRepository _photoRepository;
        private readonly PostRepository _postRepository;
        private readonly GolferRepository _golferRepository;
        private readonly CourseRepository _courseRepository;
        public PhotoService(PostRepository postRepository, GolferRepository golferRepository, CourseRepository courseRepository, PhotoRepository photoRepository)
        {
            _courseRepository = courseRepository;
            _postRepository = postRepository;
            _golferRepository = golferRepository;
            _photoRepository = photoRepository;
        }

        public string GetPhotoType(int photoType)
        {
            switch (photoType)
            {
                case 1:
                    return "Avatar";
                case 2:
                    return "Cover";
                case 3:
                    return "Post";
                case 4:
                    return "Scorecard";
                case 5:
                    return "Course";
                default:
                    throw new Exception("Can't find type of photo");
            }
        }

        public List<Photo> GetPhotos(List<string> PhotoIDs)
        {
            return _photoRepository.Find(photo => PhotoIDs.Contains(photo.Name)).ToList();
        }

        private static string GetMD5HashFromFile(MemoryStream ms)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(ms.ToArray())).Replace("-", string.Empty);
            }
        }
        /// <summary>
        /// Lưu nhiều ảnh lên serverr
        /// </summary>
        /// <param name="OwnerID">Id người đăng</param>
        /// <param name="files">Dữ liệu danh sách ảnh</param>
        /// <param name="type">Loại ảnh</param>
        /// <returns></returns>
        public async Task<List<Photo>> SavePhotos(Guid OwnerID, List<IFormFile> files, PhotoType type)
        {
            var photos = new List<Photo>();

            foreach (IFormFile file in files)
            {
                photos.Add(await SafeSavePhoto(OwnerID, file, type));
            }
            return photos;
        }
        /// <summary>
        /// Lưu 1 ảnh
        /// </summary>
        /// <param name="OwnerID">Id người đăng</param>
        /// <param name="files">Dữ liệu ảnh</param>
        /// <param name="type">Loại ảnh</param>
        /// <returns></returns>
        public async Task<Photo> SafeSavePhoto(Guid OwnerID, IFormFile file, PhotoType type)
        {
            var photo = new Photo();
            var ms = new MemoryStream();
            file.CopyTo(ms);
            string fileName = photo.ID.ToString() + GetMD5HashFromFile(ms) + Path.GetExtension(file.FileName);
            if (file.Length > 0)
            {
                var dir = Directory.GetCurrentDirectory() + @"/wwwroot/Photos/";
                var currentPhoto = _photoRepository.Find(p => p.Name == fileName && type == p.Type).FirstOrDefault();
                if (currentPhoto == null)
                {
                    var filePath = Path.Combine(dir, fileName);
                    using (var stream = File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                        photo.Name = fileName;
                        photo.Type = type;
                        photo.CreatedBy = OwnerID;
                        photo.CreatedDate = DateTime.Now;

                        _photoRepository.SafeAdd(photo);
                        return photo;
                    }
                }
                return currentPhoto;
            }
            throw new Exception("Can't save photo");
        }
        public async Task<Photo> SavePhoto(Guid OwnerID, IFormFile file, PhotoType type)
        {
            var photo = new Photo();
            var ms = new MemoryStream();
            file.CopyTo(ms);
            string fileName = photo.ID.ToString() + GetMD5HashFromFile(ms) + Path.GetExtension(file.FileName);
            if (file.Length > 0)
            {
                var dir = Directory.GetCurrentDirectory() + @"/wwwroot/Photos/";
                var currentPhoto = _photoRepository.Find(p => p.Name == fileName && type == p.Type).FirstOrDefault();
                if (currentPhoto == null)
                {
                    var filePath = Path.Combine(dir, fileName);
                    using (var stream = File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                        photo.Name = fileName;
                        photo.Type = type;
                        photo.CreatedBy = OwnerID;
                        photo.CreatedDate = DateTime.Now;
                        _photoRepository.Add(photo);
                        return photo;
                    }
                }
                return currentPhoto;
            }
            throw new Exception("Can't save photo");
        }

        /// <summary>
        /// Xóa ảnh
        /// </summary>
        /// <param name="ownerID">định danh người dừng</param>
        /// <param name="photoName">Tên ảnh</param>
        /// <returns></returns>
        public bool DeletePhoto(Guid ownerID, string photoName)
        {
            var golfer = _golferRepository.Get(ownerID);
            if (golfer == null)
                throw new NotFoundException("Not found golfer");
            var photo = this._photoRepository.Find(photo => photo.Name == photoName).FirstOrDefault();
            if (photo == null)
                throw new NotFoundException("Not found photo");
            switch (photo.Type)
            {
                case PhotoType.Avatar:
                    {
                        if (golfer.Avatar == photoName)
                        {
                            golfer.Avatar = "";
                            _golferRepository.UpdateEntity(golfer);
                        }
                        break;
                    }
                case PhotoType.Cover:
                    {
                        if (golfer.Cover == photoName)
                        {
                            golfer.Cover = "";
                            _golferRepository.UpdateEntity(golfer);
                        }
                        break;
                    }
                case PhotoType.Post:
                    {
                        var posts = _postRepository.Find(p => p.OwnerID == ownerID && p.PhotoNames.Contains(photo.Name)).ToList();
                        foreach (var i in posts)
                        {
                            if ((i.Content == "" || i.Content == null) && i.GetPhotoNames().Count() == 1)
                                _postRepository.RemoveEntity(i);
                            else
                            {
                                i.RemovePhotoName(photo.Name);
                                _postRepository.UpdateEntity(i);
                            }
                        }
                        break;
                    }
                case PhotoType.Course:
                    {
                        break;
                    }
            }
            var dir = Directory.GetCurrentDirectory() + @"/wwwroot/Photos/";
            if (photo.CreatedBy == ownerID)
            {
                if (File.Exists(Path.Combine(dir, photo.Name)))
                {
                    var filePath = Path.Combine(dir, photo.Name);
                    File.Delete(filePath);
                }
            }
            _photoRepository.RemoveEntity(photo);
            return true;
        }
        public void DeletePhoto(string photoName)
        {
            var dir = Directory.GetCurrentDirectory() + @"/wwwroot/Photos/";
            if (File.Exists(Path.Combine(dir, photoName)))
            {
                var filePath = Path.Combine(dir, photoName);
                File.Delete(filePath);
            }
        }
        public void DeletePhotos(List<string> photoNames)
        {
            
            foreach(var i in photoNames)
            {
                this.DeletePhoto(i);
            }    
        }
        public void SafeDeletePhotoDatas(Guid golferID, List<string> photoNames)
        {
            foreach (var i in photoNames)
            {
                this.SafeDeletePhotoData(golferID, i);
            }
        }
        public void SafeDeletePhotoData(Guid golferID, string photoName)
        {

            var photo = this._photoRepository.Find(photo => photo.Name == photoName).FirstOrDefault();
            if (photo == null)
                return;
            if(photo.CreatedBy== golferID)
            {
                _photoRepository.SafeRemove(photo);
            } 

        }
        public bool DeletePhotos(Guid OwnerID, List<string> PhotoIDs)
        {
            var photos = GetPhotos(PhotoIDs);
            foreach (Photo photo in photos)
            {
                var dir = Directory.GetCurrentDirectory() + $"/wwwroot/Photos/";
                if (photo.CreatedBy == OwnerID)
                {
                    if (File.Exists(Path.Combine(dir, photo.Name)))
                    {
                        var filePath = Path.Combine(dir, photo.Name);
                        File.Delete(filePath);
                    }
                }
            }
            _photoRepository.SafeRemoveRange(photos);
            return true;
        }

        /// <summary>
        /// Lấy ảnh của một người dùng nào đó
        /// </summary>
        /// <param name="OwnerID">Định danh người dùng</param>
        /// <param name="startIndex">vị trí lấy đầu tiên</param>
        /// <returns></returns>
        public List<string> GetUserPhotos(Guid OwnerID, int startIndex)
        {
            return _photoRepository.Find(p => p.CreatedBy == OwnerID).Skip(startIndex).Take(Const.PageSize).Select(p => p.Name).ToList();
        }
    }
}
