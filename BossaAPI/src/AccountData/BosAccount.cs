using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Konkretny rachunek danego użytkownika BOSSy (np. akcyjny lub kontraktowy)
	/// </summary>
	public class BosAccount
	{
		/// <summary>
		/// Numer rachunku.
		/// </summary>
		public string Number { get; private set; }

		/// <summary>
		/// Czas ostatniej aktualizacji stanu tego rachunku.
		/// </summary>
		public DateTime UpdateTime { get; private set; }

		/// <summary>
		/// Środki dostępne do wypłaty.
		/// </summary>
		public decimal AvailableCash { get; private set; }
		/// <summary>
		/// Środki dostępne dla nowych zleceń (wolna gotówka + należności lub niewykorzystany depozyt).
		/// </summary>
		public decimal AvailableFunds { get; private set; }
		/// <summary>
		/// Całkowita wartość depozytu (tylko przy rachunku kontraktowym).
		/// </summary>
		public decimal? DepositValue { get; private set; }
		/// <summary>
		/// Zablokowana wartość depozytu (tylko przy rachunku kontraktowym).
		/// </summary>
		public decimal? DepositBlocked { get; private set; }
		/// <summary>
		/// Wymagana dopłata do depozytu (tylko przy rachunku kontraktowym).
		/// </summary>
		public decimal? DepositDeficit { get; private set; }
		/// <summary>
		/// Aktualna wycena wszystkich papierów wartościowych plus gotówka
		/// (dla kontraktów - liczymy wartość środków własnych, bez dźwigni).
		/// </summary>
		public decimal PortfolioValue { get; private set; }

		/// <summary>
		/// Lista papierów wartościowych na rachunku (otwartych pozycji).
		/// </summary>
		public BosPapers Papers { get; private set; }

		/// <summary>
		/// Lista złożonych zleceń (nowych, dziś zrealizowanych itp.).
		/// </summary>
		public BosOrders Orders { get; private set; }


		#region Internal library stuff

		internal readonly IBossaApi api;

		// konstruktor wywoływany w klasie BosAccounts, gdy pojawia się nowy numer rachunku 
		internal BosAccount(IBossaApi api, string number)
		{
			this.api = api;
			Number = number;
			Papers = new BosPapers(this);
			Orders = new BosOrders(this);
		}

		// aktualizacja danych obiektu po odebraniu ich z sieci
		internal void Update(DTO.AccountData data)
		{
			AvailableCash = data.AvailableCash;
			AvailableFunds = data.AvailableFunds;
			DepositDeficit = data.DepositDeficit;
			DepositBlocked = data.DepositBlocked;
			DepositValue = data.DepositValue;
			PortfolioValue = data.PortfolioValue;
			Papers.Update(data.Papers);
			UpdateTime = DateTime.Now;
		}

		#endregion
	}
}
