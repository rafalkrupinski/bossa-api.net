using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosTrades : IEnumerable<BosTrade>
	{
		/// <summary>
		/// Liczba dostępnych w historii transakcji dla tego instrumentu.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnej transakcji w historii, licząc od zera (0 = najstarsza, Count-1 = najnowsza).
		/// </summary>
		public BosTrade this[int index]
		{
			get { return list[index]; }
		}

		/// <summary>
		/// Szybki dostęp do ostatniej (najnowszej) transakcji.
		/// Zwraca null, jeśli brak historii notowań dla tego instrumentu.
		/// </summary>
		public BosTrade Last
		{
			get { return (Count > 0) ? list[Count-1] : null; }
		}

		#region Generic list stuff

		private List<BosTrade> list = new List<BosTrade>();

		// IEnumerable<BosAccount>
		public IEnumerator<BosTrade> GetEnumerator()
		{
			for (int i = 0; i < Count; i++)
				yield return this[i];
		}

		// IEnumerable
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Internal library stuff

		// aktualizacja danych obiektu po odebraniu ich z sieci
		internal void Update(DTO.MarketTradeData data)
		{
			list.Add(new BosTrade(data));
		}

		#endregion
	}
}
