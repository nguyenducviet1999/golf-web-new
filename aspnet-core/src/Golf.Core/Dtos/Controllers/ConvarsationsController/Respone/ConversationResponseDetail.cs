using Golf.Core.Common.Golfer;
using Golf.Domain.GolferData;
using Golf.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.ConvarsationsController.Respone
{
    public class ConversationResponseDetail
    {
      public Guid ID;
      public string Name;
      public  List<MinimizedGolfer> Golfers = new List<MinimizedGolfer>();
      public List<Message> messages = new List<Message>();

    }
}
