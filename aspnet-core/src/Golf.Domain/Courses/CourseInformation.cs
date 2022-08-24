using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Courses
{
    public class CourseInformation
    {
        public string Title { get; set; }
        public List<string> Info { get; set; } = new List<string>();
    }
}
