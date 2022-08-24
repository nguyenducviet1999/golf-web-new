using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Core.Dtos.Controllers.GroupAdminController.Requests
{
    public class AddGroupMembersRequest
    {
      public  List<Guid> GolferIDs { get; set; }
    }
}
