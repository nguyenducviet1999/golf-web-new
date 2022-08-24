using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Golf.Domain.Courses
{
    public class CourseExtension
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}