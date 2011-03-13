using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// podstawowych informacji nt. konkretnego zlecenia złożonego przez klienta.
	/// Używany jako "podobiekt" w OrderData (tam precyzujemy, o które zlecenie chodzi).
	/// </summary>
	public class OrderMainData
	{
		public DateTime CreateTime;
		public Instrument Instrument;
		public BosOrderSide Side;
		public PriceType PriceType;
		public decimal? PriceLimit;
		public decimal? ActivationPrice;
		public uint Quantity;
		public uint? MinimumQuantity;
		public uint? VisibleQuantity;
		public bool ImmediateOrCancel;
		public DateTime? ExpirationDate;
	}
}
