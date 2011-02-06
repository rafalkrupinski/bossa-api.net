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
    public class BosOrder
    {
        public string Id { get; private set; }
        public DateTime Date { get; private set; }
        public BosOrderSide Side { get; private set; }
        public BosInstrument Instrument { get; private set; }

        public BosPrice Price { get; private set; }
        public decimal? ActivationPrice { get; private set; }

        public uint Quantity { get; private set; }
        public uint? MinimumQuantity { get; private set; }
        public uint? VisibleQuantity { get; private set; }

        public bool ImmediateOrCancel { get; private set; }
        public DateTime? ExpirationDate { get; private set; }

        public BosOrderReport Report { get; private set; }
    }
}
