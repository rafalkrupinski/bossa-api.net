using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
    /// <summary>
    /// Podstawowa klasa biblioteki oferująca łatwy dostęp do większości funkcji API.
    /// TODO: obecnie to sam szkielet przyszłej klasy
    ///        (na razie korzystać z klasy Networking/NolClient)
    /// </summary>
    public class Bossa
    {
        public BosAccounts Accounts { get; private set; }
        public BosInstruments Instruments { get; private set; }

        public Bossa()
        {
            Accounts = new BosAccounts();
            Instruments = new BosInstruments();
        }
    }
}
