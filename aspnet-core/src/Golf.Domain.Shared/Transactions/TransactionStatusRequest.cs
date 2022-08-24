using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Transactions
{
    public enum TransactionStatusRequest
    {
        Confirm,
        Completed,
        Cancelled,
        Rejected,
        Deleted
    }
}
