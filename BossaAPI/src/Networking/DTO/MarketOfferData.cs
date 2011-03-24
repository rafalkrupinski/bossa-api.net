using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki) aktualizacji stanu notowań rynkowych.
	/// Używany jako "podobiekt" w MarketData - informuje o pojedynczej zmianie w tabeli ofert dla danego instrumentu.
	/// Możliwe są trzy warianty:
	/// - wstawienie nowego "wiersza" w tabeli: Update = false, wszystkie pola wypełnione
	/// - aktualizacja istniejącego "wiersza" w tabeli: Update = true, wszystkie pola wypełnione
	/// - usunięcie istniejącego "wiersza" w tabeli: Level = nr wiersza, reszta pusta (null, zero itp.)
	/// </summary>
	public class MarketOfferData
	{
		public int Level;
		public bool Update;
		public PriceType PriceType;
		public decimal? PriceLimit;
		public uint Volume;
		public uint Count;
	}
}
