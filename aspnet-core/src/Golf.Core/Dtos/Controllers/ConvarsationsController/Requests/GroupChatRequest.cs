using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ConvarsationsController.Requests
{
   public class GroupChatRequest
    {
        public List<Guid> GolferIDs;
        public string ConversationName;
    }
}
