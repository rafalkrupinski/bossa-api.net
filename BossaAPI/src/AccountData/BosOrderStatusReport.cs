using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Raport na temat aktualnego stanu naszego zlecenia.
	/// </summary>
	public class BosOrderStatusReport
	{
		/// <summary>
		/// Czas ostatniej zmiany statusu.
		/// </summary>
		public DateTime Time { get; private set; }
		/// <summary>
		/// Aktualny status zlecenia (aktywne, wykonane, anulowane itp.)
		/// </summary>
		public BosOrderStatus Status { get; private set; }

		/// <summary>
		/// Dotychczas zrealizowana liczba walorów.
		/// </summary>
		public uint Quantity { get; private set; }
		/// <summary>
		/// Teoretyczna wartość całkowita zlecenia, po(!) uwzględenieniu prowizji.
		/// </summary>
		public decimal NetValue { get; private set; }
		/// <summary>
		/// Teoretyczna wartość całkowita prowizji naliczonej dla tego zlecenia.
		/// </summary>
		public decimal Commission { get; private set; }


		// konstruktor wywoływany po odebraniu nowych danych z sieci
		internal BosOrderStatusReport(DTO.OrderStatusData data)
		{
			Time = DateTime.Now;
			Status = data.Status;
			Quantity = data.Quantity;
			NetValue = data.NetValue;
			Commission = data.Commission;
		}

		// automatyczna konwersja na string - zwraca bieżący "Status"
		public override string ToString()
		{
			return Status.ToString();
		}
	}
}
