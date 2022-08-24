using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ConvarsationsController.Requests
{
    public class SendMessageRequest
    {
        public Guid ConversationID { get; set; }
        public string Message { get; set; }
        public List<IFormFile> PhotoFiles { get; set; }
    }
}
