using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki) aktualizacji stanu notowań rynkowych.
	/// Używany jako "podobiekt" w MarketData - informuje o danych "statystycznych" dotyczących całej sesji.
	/// </summary>
	public class MarketStatsData
	{
		public decimal? OpeningPrice;
		public decimal? OpeningTurnover;
		public decimal? LowestPrice;
		public decimal? HighestPrice;
		public decimal? ClosingPrice;
		public decimal? ClosingTurnover;
		public decimal? ReferencePrice;
		public uint? TotalVolume;
		public decimal? TotalTurnover;
	}
}
