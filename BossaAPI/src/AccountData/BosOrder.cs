using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Konkretne zlecenie złożone przez użytkownika rachunku.
	/// Na razie powinien działać odbiór informacji o bieżących zleceniach.
	/// TODO: pozostało składanie nowych zleceń, ich modyfikacja i anulowanie...
	/// </summary>
	public class BosOrder
	{
		/// <summary>
		/// Identyfikator (numer) zlecenia nadany przez Dom Maklerski.
		/// Jeśli to nowe, wysyłane stąd zlecenie - null, dopóki nie zostanie przyjęte w DM.
		/// </summary>
		public string Id { get; private set; }
		/// <summary>
		/// Data/godzina utworzenia zlecenia.
		/// </summary>
		public DateTime CreateTime { get; private set; }

		/// <summary>
		/// Instrument, którego zlecenie dotyczy.
		/// </summary>
		public BosInstrument Instrument { get; private set; }
		/// <summary>
		/// Czy to zlecenie kupna czy sprzedaży...
		/// </summary>
		public BosOrderSide Side { get; private set; }

		/// <summary>
		/// Limit ceny podany w zleceniu.
		/// </summary>
		public BosPrice Price { get; private set; }
		/// <summary>
		/// Limit aktywacji (null, jeśli bez stop'a).
		/// </summary>
		public decimal? ActivationPrice { get; private set; }

		/// <summary>
		/// Liczba papierów podana w zleceniu.
		/// </summary>
		public uint Quantity { get; private set; }
		/// <summary>
		/// Minimalna liczba papierów, jaka musi się zrealizować (albo zlecenie będzie anulowane).
		/// Podanie "MinimumQuantity == Quantity" jest jednoznaczne z typem zlecenia "WuA".
		/// </summary>
		public uint? MinimumQuantity { get; private set; }
		/// <summary>
		/// Liczba papierów ujawniana w arkuszu ofert (WUJ).
		/// </summary>
		public uint? VisibleQuantity { get; private set; }

		/// <summary>
		/// Zlecenie musi być wykonane natychmiast - chociaż na tyle, ile to możliwe... 
		/// Jeśli zabraknie przeciwstawnych zleceń, ew. "reszta" zostaje od razu anulowana (WiA).
		/// </summary>
		public bool ImmediateOrCancel { get; private set; }
		/// <summary>
		/// Ważność zlecenia (null - ważne tylko na bieżącą sesję).
		/// </summary>
		public DateTime? ExpirationDate { get; private set; }

		/// <summary>
		/// Raport na temat aktualnego stanu zlecenia (czy wykonane, w jakim zakresie itp.)
		/// </summary>
		public BosOrderStatusReport StatusReport { get; private set; }
		/// <summary>
		/// Szczegółowe raporty z wykonania kolejnych transakcji dla tego zlecenia.
		/// </summary>
		public BosOrderTradeReports TradeReports { get; private set; }

		/// <summary>
		/// Zwraca true, jeśli bieżący status zlecenia oznacza, że zlecenie jest aktywne
		/// (jeszcze nie wykonane albo wykonane częściowo i wciąż coś w arkuszu zleceń zostało).
		/// </summary>
		public bool IsActive
		{
			get
			{
				if (StatusReport == null) return false;
				switch (StatusReport.Status)
				{
					case BosOrderStatus.Active:
					case BosOrderStatus.ActiveFilled:
					case BosOrderStatus.PendingReplace:
					case BosOrderStatus.PendingCancel: return true;
					default: return false;
				}
			}
		}

		// konstruktor wywoływany w klasie BosOrders, gdy pojawia się nowy numer zlecenia
		internal BosOrder(DTO.OrderData data)
		{
			Id = data.BrokerId;
			Update(data);
		}

		// aktualizacja danych obiektu po odebraniu ich z sieci
		internal void Update(DTO.OrderData data)
		{
			if (data.MainData != null) Update(data.MainData);
			if (data.StatusReport != null) Update(data.StatusReport);
			if (data.TradeReport != null) Update(data.TradeReport);
		}

		// aktualizacja danych z sieci - podstawowe dane zlecenia
		private void Update(DTO.OrderMainData data)
		{
			CreateTime = data.CreateTime;
			Instrument = BosInstrument.Find(data.Instrument);
			Side = data.Side;
			Price = BosPrice.Create(data.PriceType, data.PriceLimit);
			ActivationPrice = data.ActivationPrice;
			Quantity = data.Quantity;
			MinimumQuantity = data.MinimumQuantity;
			VisibleQuantity = data.VisibleQuantity;
			ImmediateOrCancel = data.ImmediateOrCancel;
			ExpirationDate = data.ExpirationDate;
		}

		// aktualizacja danych z sieci - nowy status zlecenia
		private void Update(DTO.OrderStatusData data)
		{
			StatusReport = new BosOrderStatusReport(data);
		}

		// aktualizacja danych z sieci - nowa transakcja do tego zlecenia
		private void Update(DTO.OrderTradeData data)
		{
			if (TradeReports == null)
				TradeReports = new BosOrderTradeReports();
			TradeReports.Update(data);
		}
	}
}
