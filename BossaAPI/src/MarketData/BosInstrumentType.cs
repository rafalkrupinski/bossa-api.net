using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Typ wyliczeniowy określający rodzaj instrumentu.
	/// </summary>
	public enum BosInstrumentType
	{
		/// <summary>
		/// Instrumenty pierwotne (zwykłe akcje).
		/// </summary>
		Default,
		/// <summary>
		/// Instrumenty pochodne (m.in. kontrakty).
		/// </summary>
		Futures,
		/// <summary>
		/// Indeksy giełdowe (np. WIG, WIG20 itp.)
		/// </summary>
		Index
	}
}
