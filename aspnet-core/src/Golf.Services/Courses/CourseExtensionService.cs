using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Golf.Services.Courses
{
    public class CourseExtensionService
    {
        readonly private CourseExtensionRepository _courseExtensionRepository;

        public CourseExtensionService(CourseExtensionRepository courseExtensionRepository)
        {
            _courseExtensionRepository = courseExtensionRepository;
        }

        /// <summary>
        /// Lấy danh sách các các dịch vụ của sân
        /// </summary>
        /// <param name="extensionIDs"></param>
        /// <returns></returns>
       public List<CourseExtension> GetExtensionByListID(List<int> extensionIDs)
        {
            var result = _courseExtensionRepository.Find(ce => extensionIDs.Contains(ce.ID));
            if (result.Count()>0)
            {
                return result.ToList();
            }
            else
            {
                throw new BadRequestException("Not found couesereview !");
            }
        }
    }
}
