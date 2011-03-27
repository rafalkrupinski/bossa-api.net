using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	public class BosOffers : IEnumerable<BosOffer>
	{
		/// <summary>
		/// Liczba widocznych w tabeli ofert kupna/sprzedaży danego instrumentu.
		/// Zależnie od pakietu będzie to max: 1, 3, 5. Pełnego arkusza API chyba nie przewiduje.
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Dostęp do konkretnego "wiersza" tej tabeli (0..4, gdzie 0 = oferta najlepsza).
		/// </summary>
		public BosOffer this[int index]
		{
			get { return list[index]; }
		}

		/// <summary>
		/// Szybki dostęp do najlepszej oferty w tej tabeli (jej pierwszy "wiersz").
		/// Zwraca null, jeśli brak ofert (lub jeszcze ich nie dostaliśmy z serwera).
		/// </summary>
		public BosOffer Best
		{
			get { return (Count > 0) ? list[0] : null; }
		}

		/// <summary>
		/// Szybki dostęp do najlepszej oferty (cena z pierwszego "wiersza" tabeli).
		/// Zwraca null, jeśli brak ofert (lub jeszcze ich nie dostaliśmy z serwera).
		/// </summary>
		public BosPrice BestPrice
		{
			get { return (Count > 0) ? Best.Price : null; }
		}

		#region Generic list stuff

		private List<BosOffer> list = new List<BosOffer>();

		// IEnumerable<BosAccount>
		public IEnumerator<BosOffer> GetEnumerator()
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
		internal void Update(DTO.MarketOfferData data)
		{
			//TODO: NOL3 na razie przysyła zawsze Level=1 i bez określenia nowy/zmiana.
			data.Update = (Count > 0);

			if (data.Level > (data.Update ? Count : Count+1))
			{
				MyUtil.PrintWarning(string.Format("BosOffers.Update - unexpected level: {0} (current max: {1})", data.Level, Count));
				while (list.Count < data.Level) list.Add(null);
			}
			var n = data.Level - 1;
			if (data.Volume > 0)
			{
				var offer = new BosOffer(data);
				if (data.Update) 
					list[n] = offer;
				else
					list.Insert(n, offer);
			}
			else list.RemoveAt(n);
		}

		internal void Combine(BosOffers source)
		{
			if (list.Count == 0 && source.list.Count > 0)
				list = source.list;
		}

		#endregion
	}
}
