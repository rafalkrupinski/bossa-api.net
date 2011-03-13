using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pjank.BossaAPI.DTO;
using System.ComponentModel;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Podstawowa klasa biblioteki oferująca łatwy dostęp do większości funkcji API.
	/// TODO: wiele rzeczy tu jeszcze brakuje, na razie lepiej korzystać z klasy Networking/NolClient
	/// </summary>
	public static class Bossa
	{
		/// <summary>
		/// Czy jesteśmy połączeni z serwerem (wywołano wcześniej "Connect")?
		/// Jeśli nie, wszelkie operacje które wymagają tego połączenia, zwrócą teraz wyjątek.
		/// </summary>
		public static bool IsConnected
		{
			get { return (client != null); }
		}

		/// <summary>
		/// Dostęp do naszych rachunków w biurze maklerskim Bossa
		/// (ich saldo, obecne papiery wartościowe, bieżące zlecenia)
		/// </summary>
		public static BosAccounts Accounts { get; private set; }

		/// <summary>
		/// Dostęp do informacji o notowaniach poszczególnych instrumentów na rynku
		/// (historia ostatnich transakcji, bieżąca tabela ofert kupna/sprzedaży)
		/// </summary>
		public static BosInstruments Instruments { get; private set; }


		// inicjalizacja klasy
		static Bossa()
		{
			Accounts = new BosAccounts();
			Instruments = new BosInstruments();
		}

		/// <summary>
		/// Zdarzenie wywoływane po każdej aktualizacji danych.
		/// Automatycznie przenosi zdarzenie do wątku odbiorcy, jeśli zajdzie taka potrzeba (BeginInvoke).
		/// Jako parametr "source" przekazywany jest obiekt, który uległ zaktualizowaniu.
		/// </summary>
		public static event EventHandler OnUpdate;

		// wywołanie handlerów podpiętych pod zdarzenie OnUpdate
		private static void DoUpdate(object updatedObject)
		{
			if (OnUpdate != null)
			{
				var args = new[] { updatedObject };
				foreach (var handler in OnUpdate.GetInvocationList())
				{
					ISynchronizeInvoke invokeTarget = handler.Target as ISynchronizeInvoke;
					if (invokeTarget != null)
						invokeTarget.BeginInvoke(handler, args);
					else
						handler.DynamicInvoke(args);
				}
			}
		}

		// obiekt realizujący komunikację z serwerem
		private static IBosClient client;

		// aktualizacja stanu jednego z rachunków
		private static void AccountUpdateHandler(AccountData acccountData)
		{
			var account = Accounts[acccountData.Number];
			account.Update(acccountData);
			DoUpdate(account);
		}

		// aktualizacja informacji o bieżących zleceniach
		private static void OrderUpdateHandler(OrderData orderData)
		{
			var account = Accounts[orderData.AccountNumber];
			account.Orders.Update(orderData);
			DoUpdate(account);
		}

		/// <summary>
		/// Podłączenie wskazanego obiektu komunikującego się z serwerem.
		/// </summary>
		/// <param name="client">Obiekt realizujący konkretną formę komunikacji.
		/// Jedyna dostępna na tę chwilę implementacja tego interfejsu to klasa "NolClient".
		/// </param>
		public static void Connect(IBosClient client)
		{
			Bossa.client = client;
			client.AccountUpdateEvent += new Action<AccountData>(AccountUpdateHandler);
			client.OrderUpdateEvent += new Action<OrderData>(OrderUpdateHandler);
		}

		/// <summary>
		/// Otwarcie połączenia  lokalnie uruchomioną aplikacją NOL3(Bossa)
		/// </summary>
		public static void ConnectNOL3()
		{
			Connect(new NolClient());
		}

		/// <summary>
		/// Zamknięcie bieżącego połączenia.
		/// Wszelkie dane (stan rachunku, notowania) jakie zdążyliśmy zebrać, zostają nadal w pamięci...
		/// i można z nich korzystać (tylko odczyt). Aby wyczyścić wszystkie dane, używamy metody "Reset".
		/// </summary>
		public static void Disconnect()
		{
			Bossa.client.Dispose();
			Bossa.client = null;
		}

		/// <summary>
		/// Wyczyszczenie zebranych dotąd informacji o stanie naszych rachunków, historii notowań itd.
		/// </summary>
		public static void Reset()
		{
			Accounts.Clear();
			Instruments.Clear();
		}
	}
}
