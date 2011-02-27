using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Konkretny rachunek danego użytkownika BOSSy (np. akcyjny lub kontraktowy)
	/// TODO: ... 
	/// </summary>
	public class BosAccount
	{
		/// <summary>
		/// numer rachunku
		/// </summary>
		public string Number { get; private set; }

		/// <summary>
		/// czas ostatniej aktualizacji stanu tego rachunku
		/// </summary>
		public DateTime UpdateTime { get; private set; }

		/// <summary>
		/// stan wolnych środków na rachunku
		/// </summary>
		public decimal Cash { get; protected internal set; }

		/// <summary>
		/// lista papierów wartościowych na rachunku (otwartych pozycji)
		/// </summary>
		public BosPapers Papers { get; private set; }

		/// <summary>
		/// lista złożonych zleceń (nowych, dziś zrealizowanych itp.)
		/// </summary>
		public BosOrders Orders { get; private set; }


		// konstruktor, tylko do użytku lokalnego 
		protected internal BosAccount(string number)
		{
			Number = number;
			Papers = new BosPapers();
			Orders = new BosOrders();
		}

		internal void Update(DTO.Account dtoAccount)
		{
			Cash = dtoAccount.Cash;
			Papers.Update(dtoAccount.Papers);
			UpdateTime = DateTime.Now;
		}
	}
}
