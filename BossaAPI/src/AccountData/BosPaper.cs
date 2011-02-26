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
		/// liczba sztuk tego instrumentu znajdujących się na rachunku
		/// </summary>
		public int Quantity { get; private set; }

	}
}
