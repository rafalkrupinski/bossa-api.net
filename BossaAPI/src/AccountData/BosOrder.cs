using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pjank.BossaAPI.DTO;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Konkretne zlecenie złożone przez użytkownika rachunku.
	/// Zarówno zlecenia wciąż aktywne, te wykonane, jak i odrzucone/anulowane
	/// (z bieżącego dnia - bossaAPI nie udostępnia /na razie/ danych historycznych).
	/// </summary>
	public class BosOrder
	{
		/// <summary>
		/// Rachunek, na którym złożono to zlecenie.
		/// </summary>
		public readonly BosAccount Account;

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

		#region Internal library stuff

		// konstruktor wywoływany w klasie BosOrders, gdy pojawia się nowy numer zlecenia
		internal BosOrder(BosAccount account, OrderData data)
		{
			Account = account;
			Id = data.BrokerId;
			Update(data);
		}

		// aktualizacja danych obiektu po odebraniu ich z sieci
		internal void Update(OrderData data)
		{
			if (data.MainData != null) Update(data.MainData);
			if (data.StatusReport != null) Update(data.StatusReport);
			if (data.TradeReport != null) Update(data.TradeReport);
		}

		// aktualizacja danych z sieci - podstawowe dane zlecenia
		private void Update(OrderMainData data)
		{
			CreateTime = data.CreateTime;
			Instrument = BosInstrument.Create(data.Instrument);
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
		private void Update(OrderStatusData data)
		{
			StatusReport = new BosOrderStatusReport(data);
		}

		// aktualizacja danych z sieci - nowa transakcja do tego zlecenia
		private void Update(OrderTradeData data)
		{
			if (TradeReports == null)
				TradeReports = new BosOrderTradeReports();
			TradeReports.Update(data);
		}

		// przygotowanie obiektu transportowego na podstawie bieżącego obiektu
		private OrderData GetData()
		{
			var data = new OrderData();
			data.AccountNumber = Account.Number;
			data.BrokerId = Id;
			data.MainData = new OrderMainData();
			data.MainData.CreateTime = CreateTime;
			data.MainData.Instrument = Instrument.Convert();
			data.MainData.Side = Side;
			data.MainData.PriceType = Price.Type;
			data.MainData.PriceLimit = Price.NumValue;
			data.MainData.ActivationPrice = ActivationPrice;
			data.MainData.Quantity = Quantity;
			data.MainData.MinimumQuantity = MinimumQuantity;
			data.MainData.VisibleQuantity = VisibleQuantity;
			data.MainData.ImmediateOrCancel = ImmediateOrCancel;
			data.MainData.ExpirationDate = ExpirationDate;
			return data;
		}

		#endregion

		#region Order cancel method...

		/// <summary>
		/// Wysłanie do systemu prośby o anulowanie tego zlecenia.
		/// </summary>
		public void Cancel()
		{
			Bossa.client.OrderCancel(GetData());
		}

		#endregion

		#region Order replace methods...

		/// <summary>
		/// Wysłanie do systemu prośby o modyfikację tego zlecenia (wszystkie możliwe do zmiany parametry).
		/// </summary>
		/// <param name="newPrice">nowy limit ceny</param>
		/// <param name="newExpirationDate">nowa data ważności</param>
		public void Modify(BosPrice newPrice, DateTime? newExpirationDate)
		{
			var data = GetData();
			data.MainData.PriceType = newPrice.Type;
			data.MainData.PriceLimit = newPrice.NumValue;
			data.MainData.ExpirationDate = newExpirationDate;
			Bossa.client.OrderReplace(data);
		}

		/// <summary>
		/// Wysłanie do systemu prośby o modyfikację tego zlecenia (tylko limit ceny).
		/// </summary>
		/// <param name="newPrice">nowy limit ceny</param>
		public void Modify(BosPrice newPrice)
		{
			Modify(newPrice, ExpirationDate);
		}

		/// <summary>
		/// Wysłanie do systemu prośby o modyfikację tego zlecenia (tylko data ważności).
		/// </summary>
		/// <param name="newExpirationDate">nowa data ważności</param>
		public void Modify(DateTime? newExpirationDate)
		{
			Modify(Price, newExpirationDate);
		}

		#endregion

		#region Order create methods...

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia z podanymi parametrami. 
		/// <para>Zobacz też metody klasy BosInstrument: Order, Buy, Sell - które od razu określają, 
		/// którego instrumentu dane zlecenie ma dotyczyć i ewentualnie prezyzują też stronę transakcji (kupno/sprzedaż)</para>
		/// </summary>
		/// <param name="account">Rachunek, na który zostaje przeznaczone to zlecenie.</param>
		/// <param name="instrument">Instrument, którego walory chcemy kupić/sprzedać.</param>
		/// <param name="side">Zlecenie kupna (BosOrderSide.Buy) czy sprzedaży (BosOrderSide.Sell).</param>
		/// <param name="price">Limit ceny, jaki wstawiamy do zlecenia (BosPrice.PKC/PCR/PCRO... lub po prostu kwota).</param>
		/// <param name="activationPrice">Ewentualny limit aktywacji zlecenia (null, jeśli aktywowane od razu, bez stop'a).</param>
		/// <param name="quantity">Liczba walorów, jaką zamierzamy kupić/sprzedać.</param>
		/// <param name="minimumQuantity">Minimalna liczba walorów, jaka musi się zrealizować, albo zlecenie będzie anulowane.
		/// Podając tutaj to samo, co w polu "quantity", uzyskujemy zlecenie typu "WuA".</param>
		/// <param name="visibleQuantity">Liczba walorów ujawniana w arkuszu ofert ("WUJ").</param>
		/// <param name="immediateOrCancel">Czy to zlecenie typu "WiA" (to, co nie wykona się natychmiast, jest od razu anulowane).</param>
		/// <param name="expirationDate">Data ważności zlecenia (null, jeśli tylko na bieżącą sesję).</param>
		public static void Create(BosAccount account, BosInstrument instrument, 
			BosOrderSide side, BosPrice price, decimal? activationPrice,
			uint quantity, uint? minimumQuantity, uint? visibleQuantity, bool immediateOrCancel, DateTime? expirationDate)
		{
			var data = new OrderData();
			data.AccountNumber = account.Number;
			data.MainData = new OrderMainData();
			data.MainData.CreateTime = DateTime.Now;
			data.MainData.Instrument = instrument.Convert();
			data.MainData.Side = side;
			data.MainData.PriceType = price.Type;
			data.MainData.PriceLimit = price.NumValue;
			data.MainData.ActivationPrice = activationPrice;
			data.MainData.Quantity = quantity;
			data.MainData.MinimumQuantity = minimumQuantity;
			data.MainData.VisibleQuantity = visibleQuantity;
			data.MainData.ImmediateOrCancel = immediateOrCancel;
			data.MainData.ExpirationDate = expirationDate;
			Bossa.client.OrderCreate(data);
			// TODO: Zastanawiam się jeszczcze m.in. co z ClientId, TradeDate... i czy w ogóle byłby sens
			// od razu tworzyć taki nowy obiekt BosOrder (zamiast zaczekać aż sam się doda przy OrderUpdate).
		}

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia z podanymi parametrami. W tej wersji metody numer rachunku,
		/// na który zostaje przeznaczone to zlecenie, wybierany jest automatycznie na podstawie typu instrumentu.
		/// <para>Zobacz też metody klasy BosInstrument: Order, Buy, Sell - które od razu określają, 
		/// którego instrumentu dane zlecenie ma dotyczyć i ewentualnie prezyzują też stronę transakcji (kupno/sprzedaż)</para>
		/// </summary>
		/// <param name="instrument">Instrument, którego walory chcemy kupić/sprzedać.</param>
		/// <param name="side">Zlecenie kupna (BosOrderSide.Buy) czy sprzedaży (BosOrderSide.Sell).</param>
		/// <param name="price">Limit ceny, jaki wstawiamy do zlecenia (BosPrice.PKC/PCR/PCRO... lub po prostu kwota).</param>
		/// <param name="activationPrice">Ewentualny limit aktywacji zlecenia (null, jeśli aktywowane od razu, bez stop'a).</param>
		/// <param name="quantity">Liczba walorów, jaką zamierzamy kupić/sprzedać.</param>
		/// <param name="minimumQuantity">Minimalna liczba walorów, jaka musi się zrealizować, albo zlecenie będzie anulowane.
		/// Podając tutaj to samo, co w polu "quantity", uzyskujemy zlecenie typu "WuA".</param>
		/// <param name="visibleQuantity">Liczba walorów ujawniana w arkuszu ofert ("WUJ").</param>
		/// <param name="immediateOrCancel">Czy to zlecenie typu "WiA" (to, co nie wykona się natychmiast, jest od razu anulowane).</param>
		/// <param name="expirationDate">Data ważności zlecenia (null, jeśli tylko na bieżącą sesję).</param>
		public static void Create(BosInstrument instrument,
			BosOrderSide side, BosPrice price, decimal? activationPrice,
			uint quantity, uint? minimumQuantity, uint? visibleQuantity, bool immediateOrCancel, DateTime? expirationDate)
		{
			var account = Bossa.Accounts[instrument.Type];
			Create(account, instrument, side, price, activationPrice,
				quantity, minimumQuantity, visibleQuantity, immediateOrCancel, expirationDate);
		}

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia z podanymi parametrami (pozostałe przyjmują 
		/// domyślną wartość null/false, a numer rachunku wyznaczany jest na podstawie typu instrumentu).
		/// <para>Zobacz też metody klasy BosInstrument: Order, Buy, Sell - które od razu określają, 
		/// którego instrumentu dane zlecenie ma dotyczyć i ewentualnie prezyzują też stronę transakcji (kupno/sprzedaż)</para>
		/// </summary>
		/// <param name="instrument">Instrument, którego walory chcemy kupić/sprzedać.</param>
		/// <param name="side">Zlecenie kupna (BosOrderSide.Buy) czy sprzedaży (BosOrderSide.Sell).</param>
		/// <param name="price">Limit ceny, jaki wstawiamy do zlecenia (BosPrice.PKC/PCR/PCRO... lub po prostu kwota).</param>
		/// <param name="activationPrice">Ewentualny limit aktywacji zlecenia (null, jeśli aktywowane od razu, bez stop'a).</param>
		/// <param name="quantity">Liczba walorów, jaką zamierzamy kupić/sprzedać.</param>
		/// <param name="expirationDate">Data ważności zlecenia (null, jeśli tylko na bieżącą sesję).</param>
		public static void Create(BosInstrument instrument,
			BosOrderSide side, BosPrice price, decimal? activationPrice, uint quantity, DateTime? expirationDate)
		{
			Create(instrument, side, price, activationPrice, quantity, null, null, false, expirationDate);
		}

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia z podanymi parametrami (pozostałe przyjmują 
		/// domyślną wartość null/false, a numer rachunku wyznaczany jest na podstawie typu instrumentu).
		/// <para>Zobacz też metody klasy BosInstrument: Order, Buy, Sell - które od razu określają, 
		/// którego instrumentu dane zlecenie ma dotyczyć i ewentualnie prezyzują też stronę transakcji (kupno/sprzedaż)</para>
		/// </summary>
		/// <param name="instrument">Instrument, którego walory chcemy kupić/sprzedać.</param>
		/// <param name="side">Zlecenie kupna (BosOrderSide.Buy) czy sprzedaży (BosOrderSide.Sell).</param>
		/// <param name="price">Limit ceny, jaki wstawiamy do zlecenia (BosPrice.PKC/PCR/PCRO... lub po prostu kwota).</param>
		/// <param name="activationPrice">Ewentualny limit aktywacji zlecenia (null, jeśli aktywowane od razu, bez stop'a).</param>
		/// <param name="quantity">Liczba walorów, jaką zamierzamy kupić/sprzedać.</param>
		public static void Create(BosInstrument instrument, BosOrderSide side, BosPrice price, decimal? activationPrice, uint quantity)
		{
			Create(instrument, side, price, activationPrice, quantity, null, null, false, null);
		}

		/// <summary>
		/// Wysłanie do systemu nowego zlecenia z podanymi parametrami (pozostałe przyjmują 
		/// domyślną wartość null/false, a numer rachunku wyznaczany jest na podstawie typu instrumentu).
		/// <para>Zobacz też metody klasy BosInstrument: Order, Buy, Sell - które od razu określają, 
		/// którego instrumentu dane zlecenie ma dotyczyć i ewentualnie prezyzują też stronę transakcji (kupno/sprzedaż)</para>
		/// </summary>
		/// <param name="instrument">Instrument, którego walory chcemy kupić/sprzedać.</param>
		/// <param name="side">Zlecenie kupna (BosOrderSide.Buy) czy sprzedaży (BosOrderSide.Sell).</param>
		/// <param name="price">Limit ceny, jaki wstawiamy do zlecenia (BosPrice.PKC/PCR/PCRO... lub po prostu kwota).</param>
		/// <param name="quantity">Liczba walorów, jaką zamierzamy kupić/sprzedać.</param>
		public static void Create(BosInstrument instrument, BosOrderSide side, BosPrice price, uint quantity)
		{
			Create(instrument, side, price, null, quantity, null, null, false, null);
		}

		#endregion
	}
}
