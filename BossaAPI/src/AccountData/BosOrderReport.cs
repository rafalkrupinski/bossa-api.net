using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    /// <summary>
    /// Konkretne zlecenie złożone przez użytkownika rachunku.
    /// TODO: ...
    /// </summary>
    public class BosOrderReport
    {
        public string Id { get; private set; }

        public BosOrderStatus Status { get; private set; }

        public DateTime? LastTradeTime { get; private set; }
        public decimal? LastTradePrice { get; private set; }
        public uint? LastTradeQuantity { get; private set; }

        public uint? ExecutedQuantity { get; private set; }

        public decimal? NetValue { get; private set; }
        public decimal? Commission { get; private set; }
    }
}
