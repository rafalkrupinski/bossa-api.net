using System;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Podstawowa klasa biblioteki oferująca łatwy dostęp do większości funkcji API -
	/// za pośrednictwem wywoływanych bezpośrednio statycznych metod i właściwości.
	/// Uwaga: jeśli przeszkadza Ci taki model statyczny - patrz: klasa <c>BossaApi</c>.
	/// </summary>
	/// <example>
	/// 
	/// <code>
	///   // nawiązanie połączenia z aplikacją NOL3
	///   Bossa.ConnectNOL3();
	/// </code>
	/// <para>
	/// Najpierw niestety należy się zalogować do rachunku na stronie www i uruchomić NOL'a
	/// (wszystko na tym samym komputerze). W przyszłości mogą się pojawić inne metody połączenia
	/// - wtedy będziemy korzystali z innej metody "Connect", a cała reszta pozostanie bez zmian.
	/// Po pomyślnym połączeniu powinniśmy zaraz otrzymać aktualny stan wszystkich naszych rachunków
	/// (dostępne spod <c>Bossa.Accounts[]</c>), jak i bieżące notowania wszystkich instrumentów,
	/// jakie się znajdowały na tych rachunkach (dostępne spod <c>Bossa.Instruments[]</c>).
	/// </para>
	/// 
	/// <code>
	///   // odczyt stanu rachunku
	///   var stan_konta = Bossa.Accounts["nr-rachunku"].PortfolioValue;
	///   var wolne_srodki = Bossa.Accounts["nr-rachunku"].AvailableFunds;
	/// </code>
	/// <para>
	/// Poza ogólnymi kwotami jw. (pełen wykaz - patrz opis klasy <c>BosAccount</c>), możemy
	/// też sprawdzić stan poszczególnych papierów wartościowych znajdujących się na tym rachunku
	/// (obiekty klasy <c>BosPaper</c> dostępne spod właściwości <c>Papers[]</c>) oraz odczytać 
	/// aktualny stan wszystkich bieżących zleceń (obiekty <c>BosOrder</c> spod <c>Orders[]</c>).
	/// </para>
	/// <code>
	///   // sprawdzenie liczby otwartych pozycji danego kontraktu
	///   // (przy okazji: jak w miejsce numeru rachunku wystarczy podać jego fragment)
	///   var ile = Bossa.Accounts["00-22-"].Papers["FW20M11"].Quantity;
	/// </code>
	/// <code>
	///   // wylistowanie aktywnych zleceń z danego rachunku
	///   foreach (var order in Bossa.Accounts["00-55-"].Orders)
	///     if (order.IsActive)
	///       Console.WriteLine("{0}: {1} {2} x {3} - {4}", order.Instrument, 
	///         order.Side, order.Quantity, order.Price, order.StatusReport);
	/// </code>
	/// <para>
	/// W przykładzie wymieniono tylko najważniejsze z właściwości klasy <c>BosOrder</c>... 
	/// w rzeczywistości mamy dostęp do praktycznie wszystkich opcji zleceń obsługiwanych
	/// przez Bossę (za wyjątkiem póki co zleceń OTP i DDM+, ale i to się w końcu pojawi),
	/// jak i szczegółowych raportów ich wykonania (również kolejne cząstkowe transakcje).
	/// Z poziomu obiektu <c>BosOrder</c> możemy też zmodyfikować lub anulować dane zlecenie.
	/// </para>
	/// 
	/// <code>
	///   // szybki odczyt kursu danego instrumentu
	///   var kurs = Bossa.Instruments["KGHM"].Trades.LastPrice;
	/// </code>
	/// <para>
	/// Za pośrednictwem <c>Bossa.Instruments</c> mamy dostęp do wszystkich notowań rynkowych.
	/// Nie musimy tu nic deklarować, ustawiać żadnych "filtrów"... wystarczy się tylko odwołać
	/// do interesującego nas instrumentu. A jeśli jeszcze takiego nie było, zostanie stworzony
	/// (i od tego też momentu biblioteka automatycznie odbiera notowania dla tego instrumentu).
	/// </para>
	/// <code>
	///   // tym razem po kodzie ISIN zamiast Symbolu - możemy ich używać zamiennie
	///   var kghm = Bossa.Instruments.FindByISIN("PLKGHM000017");
	///   // odczyt całej znanej historii notowań
	///   foreach (var trade in kghm.Trades)
	///     Console.WriteLine("{0}  {1,7} x {2,-5}",
	///       trade.Time.TimeOfDay, trade.Price, trade.Quantity);
	///   // odczyt najlepszych pozycji w tabeli ofert
	///   var bid = kghm.BuyOffers.BestPrice;
	///   var ask = kghm.SellOffers.BestPrice;
	/// </code>
	/// <para>
	/// Przy pierwszym odwołaniu do danego instrumentu może się zdarzyć, że takie pola,
	/// jak przytoczone w przykładzie: LastPrice, BestPrice - zwrócą "null". Wystarczy
	/// wtedy chwilę zaczekać, aż otrzymamy pierwszą aktualizację danych rynkowych 
	/// (albo jeszcze lepiej - podłączyć się pod zdarzenie <c>Bossa.OnUpdate</c>).
	/// </para>
	/// 
	/// <code>
	///   // złożenie zlecenia kupna 10 sztuk po 175.50 zł
	///   Bossa.Instruments["KGHM"].Buy(175.50, 10);
	///   // zlecenie sprzedaży 10 sztuk StopLoss z aktywacją po 170.00 zł
	///   Bossa.Instruments["KGHM"].Sell(BosPrice.PKC, 170, 10);
	/// </code>
	/// <para>
	/// O ile wspomniana wcześniej klasa <c>BosOrder</c> również posiada (statyczne) metody 
	/// do generowania nowych zleceń - dużo wygodniej jest użyć jednej z dostępnych metod na
	/// konkretnej instancji klasy <c>BosInstrument</c>. Mamy do dyspozycji po kilka wariantów
	/// metod "Buy" i "Sell", jak i bardziej ogólne "Order". Składać można zlecenia z limitem
	/// aktywacji (jak w przykładzie), z przedłużoną ważnością, w trybie WiA, z WUJ itp. itd.
	/// </para>
	/// <para>
	/// TODO: Każda z ww. metod do składania zleceń powinna zwrocić nowy obiekt <c>BosOrder</c>, 
	/// który możemy sobie zapamiętać "na boku" i łatwo potem kontrolować stan tego konkretnego
	/// zlecenia, a nawet je zmodyfikować (metoda "Modify") albo anulować (metoda "Cancel").
	/// Póki co pozostaje samodzielne odszukanie nowego zlecenia na liście BosAccount.Orders[].
	/// </para>
	/// 
	/// </example>
	public static class Bossa
	{
		// instancja klasy, do której tak na prawdę przekazujemy wewnętrznie wszystkie operacje
		private static IBossaApi api = new BossaApi();


		/// <summary>
		/// Czy jesteśmy połączeni z serwerem (wywołano wcześniej "Connect")?
		/// Jeśli nie, wszelkie operacje które wymagają tego połączenia, zwrócą teraz wyjątek.
		/// </summary>
		public static bool Connected { get { return api.Connected; } }

		/// <summary>
		/// Dostęp do naszych rachunków w biurze maklerskim Bossa
		/// (ich saldo, obecne papiery wartościowe, bieżące zlecenia).
		/// </summary>
		public static BosAccounts Accounts { get { return api.Accounts; } }

		/// <summary>
		/// Dostęp do informacji o notowaniach poszczególnych instrumentów na rynku
		/// (historia ostatnich transakcji, bieżąca tabela ofert kupna/sprzedaży).
		/// </summary>
		public static BosInstruments Instruments { get { return api.Instruments; } }

		/// <summary>
		/// Zdarzenie wywoływane po każdej aktualizacji danych.
		/// Automatycznie przenosi zdarzenie do wątku odbiorcy, jeśli zajdzie taka potrzeba (BeginInvoke).
		/// Jako parametr "source" przekazywany jest obiekt, który uległ zaktualizowaniu
		/// (obiekt klasy "BosAccount" - jeśli zmiana dotyczy stanu rachunku, bieżących zleceń...
		/// albo "BosInstrument" - jeśli nastąpiła aktualizacja notowań dla danego instrumentu).
		/// TODO: Dokładniejsze przekazywanie w argumentach zdarzenia co konkretnie się zmieniło.
		/// </summary>
		public static event EventHandler OnUpdate 
		{
			add { api.OnUpdate += value; }
			remove { api.OnUpdate -= value; }
		}

		/// <summary>
		/// Podłączenie wskazanego obiektu komunikującego się z serwerem.
		/// </summary>
		/// <param name="client">Obiekt realizujący konkretną formę komunikacji.
		/// Jedyna dostępna na tę chwilę implementacja tego interfejsu to klasa "NolClient".
		/// </param>
		public static void Connect(IBosClient client)
		{
			api.Connect(client);
		}

		/// <summary>
		/// Otwarcie połączenia z lokalnie uruchomioną aplikacją NOL3 (Bossa).
		/// </summary>
		public static void ConnectNOL3()
		{
			api.Connect(new NolClient());
		}

		/// <summary>
		/// Zamknięcie bieżącego połączenia.
		/// Wszelkie dane (stan rachunku, notowania) jakie zdążyliśmy zebrać, zostają nadal w pamięci...
		/// i można z nich korzystać (tylko odczyt). Aby wyczyścić wszystkie dane, używamy metody "Clear".
		/// </summary>
		public static void Disconnect()
		{
			api.Disconnect();
		}

		/// <summary>
		/// Wyczyszczenie zebranych dotąd informacji o stanie naszych rachunków, historii notowań itd.
		/// </summary>
		public static void Clear()
		{
			api.Clear();
		}
	}
}
