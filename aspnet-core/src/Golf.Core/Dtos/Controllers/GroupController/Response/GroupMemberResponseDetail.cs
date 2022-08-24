using Golf.Core.Common.Golfer;
using Golf.Domain.GolferData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GroupController.Response
{
    public class GroupMemberResponseDetail
    {
       public Golfer Golfer { get; set; } = new Golfer();
       public Statistics Statistics { get; set; } = new Statistics();
    }
}
