using Golf.Core.Dtos.Controllers.CourseAdminController.Requests;
using Golf.Domain.Shared.Groups;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GroupController.Requests
{
    public class GroupRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }//Mô tả thêm
        public GroupType Type { get; set; }
        public IFormFile? PhotoFile { get; set; }
        public List<Guid> MemberIDs { get; set; } = new List<Guid>();
       
    }
    public class EditGroupRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }//Mô tả thêm
        public GroupType Type { get; set; }
    }
}
