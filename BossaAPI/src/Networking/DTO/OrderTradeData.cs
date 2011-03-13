using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI.DTO
{
	/// <summary>
	/// Obiekt transportowy do przekazywania (między modułami biblioteki)
	/// raportu o wykonaniu transakcji związanej ze zleceniem złożonym przez klienta.
	/// Używany jako "podobiekt" w OrderData (tam precyzujemy, o które zlecenie chodzi).
	/// </summary>
	public class OrderTradeData
	{
		public DateTime Time;
		public decimal Price;
		public uint Quantity;
		public decimal NetValue;
		public decimal Commission;
	}
}
