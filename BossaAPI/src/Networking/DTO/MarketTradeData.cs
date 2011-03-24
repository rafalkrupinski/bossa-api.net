using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki) aktualizacji stanu notowań rynkowych.
	/// Używany jako "podobiekt" w MarketData - informuje o realizacji nowej transakcji dla danego instrumentu.
	/// </summary>
	public class MarketTradeData
	{
		public DateTime Time;
		public decimal Price;
		public uint Quantity;
	}
}
