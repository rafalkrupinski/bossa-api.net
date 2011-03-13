using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Raport z wykonania pojedynczej transakcji dla naszego zlecenia
	/// (może to być całe zlecenie albo jedna z wielu części wykonania danego zlecenia).
	/// </summary>
	public class BosOrderTradeReport
	{
		/// <summary>
		/// Godzina realizacji danej transakcji.
		/// </summary>
		public DateTime Time { get; private set; }
		/// <summary>
		/// Cena, po jakiej zrealizowano daną transakcję.
		/// </summary>
		public decimal Price { get; private set; }
		/// <summary>
		/// Liczba walorów zrealizowana w danej transakcji.
		/// </summary>
		public uint Quantity { get; private set; }

		/// <summary>
		/// Całkowita wartość danej transakcji, po(!) uwzględenieniu prowizji.
		/// </summary>
		public decimal NetValue { get; private set; }
		/// <summary>
		/// Wartość samej prowizji pobranej przy danej transakcji.
		/// </summary>
		public decimal Commission { get; private set; }


		// konstruktor wywoływany po odebraniu nowych danych z sieci
		internal BosOrderTradeReport(DTO.OrderTradeData data)
		{
			Time = data.Time;
			Price = data.Price;
			Quantity = data.Quantity;
			NetValue = data.NetValue;
			Commission = data.Commission;
		}
	}
}
