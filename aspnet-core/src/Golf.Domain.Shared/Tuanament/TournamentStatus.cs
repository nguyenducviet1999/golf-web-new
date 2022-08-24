using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Tuanament
{
   public enum TournamentStatus
    {
        Requested,//yêu cầu tạo
        Rejected,//yêu cầu bị từ chối
        Registered//yêu cầu được chaassps nhận
    }
}
