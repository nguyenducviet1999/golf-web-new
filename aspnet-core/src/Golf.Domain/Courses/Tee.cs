using Golf.Domain.Shared.Course;
namespace Golf.Domain.Courses
{
    public class Tee
    {
        public string Name { get; set; }
        public TeeColor Color { get; set; }
        public double SlopeRating { get; set; }
        public double CourseRating { get; set; }
    }
}