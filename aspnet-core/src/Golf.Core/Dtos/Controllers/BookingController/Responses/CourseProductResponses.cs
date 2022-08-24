using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.BookingController.Responses
{
    public class CourseProductResponses
    {
        public List<ProductResponse> TopProducts { get; set; } = new List<ProductResponse>();
        public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    }
}
