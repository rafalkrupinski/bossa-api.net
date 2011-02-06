using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    /// <summary>
    /// Reprezentuje konkretną transakcję w historii notowań instrumentu. 
    /// TODO: ...
    /// </summary>
    public class BosTrade
    {
        public DateTime Time { get; private set; }
        public decimal Price { get; private set; }
        public uint Quantity { get; private set; }
        public uint? OpenInt { get; internal set; }

        internal BosTrade(DateTime time, decimal price, uint volume, uint? lop)
        {
            this.Time = time;
            this.Price = price;
            this.Quantity = volume;
            this.OpenInt = lop;
        }

        public override string ToString()
        {
            return string.Format("{2}  {0,7} x {1,-5}  {3,8:(0)}", Price, Quantity, Time.TimeOfDay, OpenInt);
        }
    }
}
