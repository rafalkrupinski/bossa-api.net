using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Przechowuje informacje "statystyczne" z bieżącej sesji dla konkretnego instrumentu. 
	/// </summary>
	public class BosSessionStats
	{
		/// <summary>
		/// Cena otwarcia bieżącej sesji.
		/// </summary>
		public decimal? OpeningPrice;
		/// <summary>
		/// Najniższa cena w bieżącej sesji.
		/// </summary>
		public decimal? LowestPrice;
		/// <summary>
		/// Najwyższa cena w bieżącej sesji.
		/// </summary>
		public decimal? HighestPrice;
		/// <summary>
		/// Cena zamknięcia bieżącej sesji.
		/// </summary>
		public decimal? ClosingPrice;
		/// <summary>
		/// Cena odniesienia (zwykle zamknięcie poprzedniej sesji).
		/// </summary>
		public decimal? ReferencePrice;

		/// <summary>
		/// Całkowity wolumen obrotu bieżącej sesji.
		/// </summary>
		public uint TotalVolume;

		/// <summary>
		/// Wartość obrotu na otwarciu bieżącej sesji.
		/// </summary>
		public decimal? OpeningTurnover;
		/// <summary>
		/// Wartość obrotu na zamknięciu bieżącej sesji.
		/// </summary>
		public decimal? ClosingTurnover;
		/// <summary>
		/// Całkowita wartość obrotu bieżącej sesji.
		/// </summary>
		public decimal TotalTurnover;


		#region Internal library stuff

		// aktualizacja danych obiektu po odebraniu ich z sieci
		internal void Update(DTO.MarketStatsData data)
		{
			if (data.OpeningPrice.HasValue)
			{
				OpeningPrice = data.OpeningPrice;
				OpeningTurnover = data.OpeningTurnover;
			}
			if (data.ClosingPrice.HasValue)
			{
				ClosingPrice = data.ClosingPrice;
				ClosingTurnover = data.ClosingTurnover;
			}
			if (data.TotalVolume.HasValue)
			{
				TotalVolume = (uint)data.TotalVolume;
				TotalTurnover = (decimal)data.TotalTurnover;
			}
			if (data.HighestPrice.HasValue) HighestPrice = data.HighestPrice;
			if (data.LowestPrice.HasValue) LowestPrice = data.LowestPrice;
			if (data.ReferencePrice.HasValue) ReferencePrice = data.ReferencePrice;
		}

		internal void Combine(BosSessionStats source)
		{
			if (OpeningPrice.HasValue) return;
			OpeningPrice = source.OpeningPrice;
			OpeningTurnover = source.OpeningTurnover;
			ClosingPrice = source.ClosingPrice;
			ClosingTurnover = source.ClosingTurnover;
			TotalVolume = source.TotalVolume;
			TotalTurnover = source.TotalTurnover;
			HighestPrice = source.HighestPrice;
			LowestPrice = source.LowestPrice;
			ReferencePrice = source.ReferencePrice;
		}

		#endregion
	}
}
