using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    public enum BosOrderStatus
    {
        New,
        Rejected,
        PendingReplace,
        PendingCancel,
        Cancelled,
        PartiallyFilled,
        Filled,
        Expired,
    }
}
