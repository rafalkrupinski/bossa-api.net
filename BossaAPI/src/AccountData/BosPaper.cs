using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Reprezentuje jeden z papierów na rachunku użytkownika (jego ilość, bieżącą wycenę itd.)
	/// </summary>
	public class BosPaper
	{
		/// <summary>
		/// Rachunek, na którym znajdują się te papiery.
		/// </summary>
		public readonly BosAccount Account;

		/// <summary>
		/// Instrument, którego dotyczy ten wpis na rachunku użytkownika.
		/// </summary>
		public BosInstrument Instrument { get; private set; }

		/// <summary>
		/// Łączna liczba sztuk tego instrumentu znajdujących się na rachunku (wolne + zablokowane).
		/// </summary>
		public int Quantity { get { return Quantity110 + Quantity120; } }
		/// <summary>
		/// Liczba sztuk znajdujących się na koncie "110" - wolne do dyspozycji (sprzedaży).
		/// </summary>
		public int Quantity110 { get; private set; }
		/// <summary>
		/// Liczba sztuk znajdujących się na koncie "120" - zablokowane (wystawione na sprzedaż).
		/// </summary>
		public int Quantity120 { get; private set; }

		#region Internal library stuff

		// konstruktor, wywoływany spod BosPapers.Update()
		internal BosPaper(BosAccount account, DTO.Paper data)
		{
			Account = account;
			Instrument = BosInstrument.Create(data.Instrument);
			Quantity110 = data.Account110;
			Quantity120 = data.Account120;
		}

		#endregion

		#region Order create methods...

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia na "oddanie" wszystkich dostępnych na rachunku papierów po wskazanej cenie
		/// (sprzedaż, jeśli to akcje lub otwarte długie pozycje... lub przeciwstawne zlecenie kupna, jeśli to otwarte krótkie pozycje).
		/// </summary>
		/// <param name="price">Limit ceny: BosPrice.PKC/PCR/PCRO... lub po prostu kwota.</param>
		public void Release(BosPrice price)
		{
			if (Quantity110 > 0)
				Instrument.Sell(price, (uint)Quantity110);
			else
				Instrument.Buy(price, (uint)-Quantity110);
		}

		#endregion
	}
}
