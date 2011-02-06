using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    /// <summary>
    /// Reprezentuje konkretną ofertę w tabeli ofert bieżących notowań instrumentu.
    /// TODO: ...
    /// </summary>
    public class BosOffer
    {
        public BosPrice Price { get; private set; }
        public uint Volume { get; private set; }
        public uint Count { get; private set; }

        internal BosOffer(decimal? price, string pricestr, uint volume, uint count)
        {
            if (price != null) this.Price = price;
            else if (pricestr == "PKC") this.Price = BosPrice.PKC;
            else if (pricestr == "PCR") this.Price = BosPrice.PCR;
            else if (pricestr == "PCRO") this.Price = BosPrice.PCRO;
            else throw new ArgumentException("Unexpected price value", "pricestr");
            this.Volume = volume;
            this.Count = count;
        }

        public override string ToString()
        {
            return string.Format("{0,7} x {1,-5} ({2})", Price, Volume, Count);
        }
    }
}
