using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Shared.Groups
{
    public enum GroupMemberAction
    {
        SetAdmin,
        SetModerator,
        RejectModerator,
        AddMember,
        DeleteMember,
        ConfirmRequest,
    }
}
