using Golf.Core.Common.Golfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.ConvarsationsController.Respone
{
   public class ConversationResponse
    {
        public string Name;
        public string Image;
        public List<MinimizedGolfer> Receivers=new List<MinimizedGolfer>();
        public string LastMessages;
        public Guid? OwnerIDLastMessages;
        public bool IsRead;
        public Guid ID;
        public DateTime LastTime;
    }
}
