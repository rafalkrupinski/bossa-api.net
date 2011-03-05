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
		/// środki dostępne do wypłaty
		/// </summary>
		public decimal AvailableCash { get; private set; }
		/// <summary>
		/// środki dostępne dla nowych zleceń (wolna gotówka + należności lub niewykorzystany depozyt)
		/// </summary>
		public decimal AvailableFunds { get; private set; }
		/// <summary>
		/// wymagana dopłata depozytu (tylko przy rachunku kontraktowym)
		/// </summary>
		public decimal? DepositDeficit { get; private set; }
		/// <summary>
		/// całkowita wartość depozytu (tylko przy rachunku kontraktowym)
		/// </summary>
		public decimal? DepositValue { get; private set; }
		/// <summary>
		/// aktualna wycena wszystkich papierów wartościowych plus gotówka
		/// (dla kontraktów - liczymy wartość środków własnych, bez dźwigni)
		/// </summary>
		public decimal PortfolioValue { get; private set; }

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
			AvailableCash = dtoAccount.AvailableCash;
			AvailableFunds = dtoAccount.AvailableFunds;
			DepositDeficit = dtoAccount.DepositDeficit;
			DepositValue = dtoAccount.DepositValue;
			PortfolioValue = dtoAccount.PortfolioValue;
			Papers.Update(dtoAccount.Papers);
			UpdateTime = DateTime.Now;
		}
	}
}
