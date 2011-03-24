using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Reprezentuje konkretną ofertę w tabeli ofert bieżących notowań instrumentu.
	/// </summary>
	public class BosOffer
	{
		/// <summary>
		/// Cena danej oferty.
		/// </summary>
		public BosPrice Price { get; private set; }
		/// <summary>
		/// Liczba walorów po danej cenie.
		/// </summary>
		public uint Volume { get; private set; }
		/// <summary>
		/// Liczba różnych zleceń po danej cenie.
		/// </summary>
		public uint Count { get; private set; }

		// konstruktor, wywoływany spod BosOffers.Update()
		internal BosOffer(DTO.MarketOfferData data)
		{
			Price = BosPrice.Create(data.PriceType, data.PriceLimit);
			Volume = data.Volume;
			Count = data.Count;
		}

		public override string ToString()
		{
			return string.Format("{0,7} x {1,-5} ({2})", Price, Volume, Count);
		}
	}
}
