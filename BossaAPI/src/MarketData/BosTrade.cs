using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Reprezentuje konkretną transakcję w historii notowań instrumentu. 
	/// </summary>
	public class BosTrade
	{
		/// <summary>
		/// Czas realizacji danej transakcji.
		/// </summary>
		public DateTime Time { get; private set; }
		/// <summary>
		/// Cena, po jakiej zrealizowano transakcję.
		/// </summary>
		public decimal Price { get; private set; }
		/// <summary>
		/// Liczba walorów, jakie zmieniły właściciela w tej transakcji.
		/// </summary>
		public uint Quantity { get; private set; }
		/// <summary>
		/// TODO: Liczba otwartych pozycji - LOP (dotyczy tylko instrumentów pochodnych).
		/// </summary>
		public uint? OpenInt { get; internal set; }

		// konstruktor, wywoływany spod BosTrades.Update()
		internal BosTrade(DTO.MarketTradeData data)
		{
			Time = data.Time;
			Price = data.Price;
			Quantity = data.Quantity;
		}

		public override string ToString()
		{
			return string.Format("{2}  {0,7} x {1,-5}  {3,8:(0)}", Price, Quantity, Time.TimeOfDay, OpenInt);
		}
	}
}
