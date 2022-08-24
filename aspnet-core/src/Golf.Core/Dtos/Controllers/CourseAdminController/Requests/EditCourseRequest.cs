using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using Golf.Domain.Courses;
using Golf.Core.Dtos.Request;

namespace Golf.Core.Dtos.Controllers.CourseAdminController.Requests
{
    public class EditCourseRequest : IRequest
    {
        public Guid LocationID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> Extensions { get; set; }
        public List<CourseInformation> MoreInformations { get; set; }

        public int TotalHoles { get; set; }
        public List<Tee> Tees { get; set; }
        public List<CourseHole> CourseHoles { get; set; }

        public override bool Validate()
        {
            if (this.TotalHoles != CourseHoles.Count)
            {
                return false;
            }
            return true;
        }
    }
}