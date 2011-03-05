using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Reprezentuje jeden z papierów na rachunku użytkownika (jego ilość, bieżącą wycenę itd.)
	/// TODO: ...
	/// </summary>
	public class BosPaper
	{
		/// <summary>
		/// instrument, którego dotyczy ten wpis na rachunku użytkownika
		/// </summary>
		public BosInstrument Instrument { get; private set; }

		/// <summary>
		/// łączna liczba sztuk tego instrumentu znajdujących się na rachunku (wolne + zablokowane)
		/// </summary>
		public int Quantity { get { return Quantity110 + Quantity120; } }
		/// <summary>
		/// liczba sztuk znajdujących się na koncie "110" - wolne do dyspozycji (sprzedaży)
		/// </summary>
		public int Quantity110 { get; private set; }
		/// <summary>
		/// liczba sztuk znajdujących się na koncie "120" - zablokowane (wystawione na sprzedaż)
		/// </summary>
		public int Quantity120 { get; private set; }

		// konstruktor, wywoływany spod BosPapers.Update()
		internal BosPaper(DTO.Paper dtoPaper)
		{
			Instrument = BosInstrument.Find(dtoPaper.Instrument);
			Quantity110 = dtoPaper.Account110;
			Quantity120 = dtoPaper.Account120;
		}
	}
}
