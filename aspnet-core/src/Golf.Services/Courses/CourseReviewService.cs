using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Golf.Services.Courses
{
    public class CourseReviewService
    {
        readonly private CourseReviewRepository _courseReviewRepository;

        public CourseReviewService(CourseReviewRepository courseReviewRepository)
        {
            _courseReviewRepository = courseReviewRepository;
        }

        /// <summary>
        /// Lấy đối tượng course review theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<CourseReview> Get(Guid id)
        {
            var result = _courseReviewRepository.Get(id);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found couesereview !");
            }
        }

        /// <summary>
        /// Lấy course review của một sân
        /// </summary>
        /// <param name="cId"></param>
        /// <returns></returns>
        async public Task<IEnumerable<CourseReview>> GetListCourseReviewByCourseId(Guid cId)
        {
            var result = _courseReviewRepository.Find(cr => cr.CourseID == cId);
            if (result != null)
            {
                return result;
            }
            else
            {
                throw new BadRequestException("Not found couesereview !");
            }
        }

        async public Task<double> GetListCourseReviewPointByCourseId(Guid cId)
        {
            var tmp = _courseReviewRepository.Find(cr => cr.CourseID == cId).ToList();
            if (tmp.Count() > 0)
            {
                var result = tmp.Average(cr => cr.Point);
                return result;
            }
            return 0;
        }

        /// <summary>
        /// Thêm course review cho một sân
        /// </summary>
        /// <param name="uId">Id người dùng review</param>
        /// <param name="courseReview">Đối tượng course review</param>
        /// <returns></returns>
        async public Task<CourseReview> Add(Guid uId, CourseReview courseReview)
        {

            courseReview.CreatedBy = uId;
            _courseReviewRepository.Add(courseReview);
            return courseReview;
        }

        /// <summary>
        /// SỬa đánh giá sân
        /// </summary>
        /// <param name="uId">Id người dùng review</param>
        /// <param name="courseReview">Đối tượng course review</param>
        /// <returns></returns>
        async public Task<CourseReview> Edit(Guid uId, CourseReview courseReview)
        {
            if (uId == (Guid)courseReview.CreatedBy)
            {
                _courseReviewRepository.UpdateEntity(courseReview);
                return _courseReviewRepository.Get(courseReview.ID);
            }
            else
            {
                throw new BadRequestException("CourseReview dosen't exit!");
            }

        }

        /// <summary>
        /// Xóa đánh giá sân
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        async public Task<CourseReview> Delete(Guid uId, Guid id)
        {
            var result = _courseReviewRepository.Get(id);
            if (result != null)
            {
                if (result.CreatedBy == uId)
                {
                    _courseReviewRepository.RemoveEntity(result);
                    return result;
                }
                else
                {
                    throw new BadRequestException("Can not delete couesereview !");
                }
            }
            else
            {
                throw new BadRequestException("This couesereview dosen't exit !");
            }
        }
    }
}
