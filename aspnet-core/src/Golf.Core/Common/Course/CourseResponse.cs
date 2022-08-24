using System;

namespace Golf.Core.Common.Course
{
    public class CourseResponse
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public string Address { get; set; }
        public string Distance { get; set; }

    }
}