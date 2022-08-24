using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Golf.Domain.Base;
using Golf.Domain.Post;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.AdminController.Report.Request
{
    public class ReportRequest
    {
        public string Content { get; set; }
        public PhotoFile? PhotoFile { get; set; }
        public ReferenceObject ReferenceObject { get; set; } = new ReferenceObject();
    }
}
