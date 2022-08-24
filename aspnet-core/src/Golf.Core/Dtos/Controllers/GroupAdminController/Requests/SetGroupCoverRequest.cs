using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Golf.Core.Dtos.Controllers.GroupAdminController.Requests
{
    public class SetGroupCoverRequest
    {
        public IFormFile? GroupCover { get; set; }
    }
}