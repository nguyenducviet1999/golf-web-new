using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared
{
    public enum TransactionStatus
    {
        Request ,
       Confirm ,
        Completed ,
       Cancelled ,
        Rejected,
    }
}
