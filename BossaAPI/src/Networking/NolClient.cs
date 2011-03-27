using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Win32;
using pjank.BossaAPI.Fixml;
using pjank.BossaAPI.DTO;
using System.Reflection;

namespace pjank.BossaAPI
{
	/// <summary>
	/// Klasa obsługująca połączenie z aplikacją NOL3 
	/// - lokalna komunikacja z wykorzystaniem protokołu FIXML
	/// - nawiązywanie połączenia, logowanie, wylogowanie
	/// - odbiór komunikatów w kanale asynchronicznym
	/// - zbieranie bieżących notowań podczas sesji
	/// </summary>
	public class NolClient : IDisposable, IBosClient
	{

		#region Constructors

		/// <summary>
		/// Domyślny konstruktor. Automatycznie loguje się do NOL'a i otwiera połączenie asynchroniczne. 
		/// </summary>
		public NolClient() : this(true) { }

		/// <summary>
		/// Dodatkowy konstruktor pozwalający wyłaczyć automatyczne zalogowanie (i obsługę kanału asynchronicznego).
		/// </summary>
		/// <param name="login">Podając "false" wyłączamy automatyczną próbę zalogowania... 
		/// możemy się zalogować potem sami wywołując jawnie metodę "Login". </param>
		public NolClient(bool login) : this(login, login) { }

		/// <summary>
		/// Dodatkowy konstruktor pozwalający wciąż automatycznie zalogować się do NOL'a 
		/// ale z ewentualnym pominięciem automatycznego otwierania kanału asynchronicznego.
		/// </summary>
		/// <param name="login">Czy ma się od razu zalogować? Jeśli "false", należy potem jawnie wywołać metodę "Login".</param>
		/// <param name="thread">Czy ma uruchomić wewnętrzną obsługę kanału asynchronicznego? Jeśli "false" (tutaj, lub przy
		/// pierwszym argumencie - oba są wymagane), o otwarcie socketu i odbiór komunikatów tam wysyłanych musimy zadbać sami.</param>
		public NolClient(bool login, bool thread)
		{
			StatementMsgEvent += new Action<StatementMsg>(StatementMsgHandler);
			ExecReportMsgEvent += new Action<ExecutionReportMsg>(ExecReportMsgHandler);
			MarketDataMsgEvent += new Action<MarketDataIncRefreshMsg>(MarketDataMsgHandler);
			UserResponseMsgEvent += new Action<UserResponseMsg>(AsyncUserResponseMsgHandler);
			FixmlInstrument.DictionaryLoad();
			if (login) Login();
			if (login && thread) ThreadStart();
		}

		#endregion

		#region IDisposable support

		private bool loggedIn = false;
		private Thread thread;

		~NolClient()
		{
			Dispose();
		}

		public void Dispose()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.Dispose...", FixmlMsg.DebugCategory);
			try
			{
				if (thread != null) ThreadStop();
				if (loggedIn) Logout();
				FixmlInstrument.DictionarySave();
			}
			catch (Exception e)
			{
				e.PrintError();
			}
			GC.SuppressFinalize(this);
		}

		#endregion



		#region NOL3 connection sockets

		// ustalenie numeru portu, na którym nasłuchuje aplikacja NOL3
		// parametr "name" określa, który z dwóch portów nas interesuje: psync, pasync
		private static int getRegistryPort(String name)
		{
			string regname = "Software\\COMARCH S.A.\\NOL3\\7\\Settings";
			RegistryKey regkey = Registry.CurrentUser.OpenSubKey(regname);
			if (regkey == null) throw new NolClientException("NOL3 registry key missing!");
			Object value = regkey.GetValue("ncaset_" + name);
			if (value == null) throw new NolClientException("NOL3 registry settings missing!");
			if (value.ToString() == "0") throw new NolClientException("NOL3 port '" + name + "' not ready!");
			value = regkey.GetValue("nca_" + name);
			if (value == null) throw new NolClientException("NOL3 port '" + name + "' not defined!");
			return int.Parse(value.ToString());
		}

		/// <summary>
		/// Otwarcie nowego połączenia z aplikacją NOL3 na porcie do komunikacji *synchronicznej*.
		/// </summary>
		/// <returns>Zwraca nowy "socket", do którego zapisujemy/odczytujemy komunikaty synchroniczne.
		/// UWAGA: NOL3 wymaga, by dla każdej nowej operacji otworzyć nowe połączenie synchroniczne.
		/// </returns>
		public static Socket GetSyncSocket()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.GetSyncSocket...", FixmlMsg.DebugCategory);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect("localhost", getRegistryPort("psync"));
			socket.ReceiveTimeout = 10000;  // <- nie mniej, niż max. czas odpowiedzi na request synchr.
			socket.SendTimeout = 10000;
			return socket;
		}

		/// <summary>
		/// Otwarcie nowego połączenia z aplikacją NOL3 na porcie do komunikacji *asynchronicznej*.
		/// </summary>
		/// <returns>Zwraca nowy "socket", z którego możemy odbierać przesyłane asynchronicznie komunikaty.
		/// UWAGA: Kanał asynchroniczny otwieramy tylko raz, *po* zalogowaniu się w aplikacji.
		/// </returns>
		public static Socket GetAsyncSocket()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.GetAsyncSocket...", FixmlMsg.DebugCategory);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect("localhost", getRegistryPort("pasync"));
			socket.ReceiveTimeout = 10000;  // <- nie mniej, niż odstęp między Heartbeat (zwykle co 1sek?)
			socket.SendTimeout = 10000;
			return socket;
		}

		#endregion


		#region Asynchronous connection thread

		// uruchomienie wątku obsługującego odbiór komunikatów z kanału asynchronicznego
		private void ThreadStart()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.ThreadStart...", FixmlMsg.DebugCategory);
			thread = new Thread(ThreadProc);
			thread.Name = "NOL3 async connection";
			thread.IsBackground = true;
			thread.Start();
		}

		// zakończenie wątku obsługującego kanał asynchroniczny
		private void ThreadStop()
		{
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.ThreadStop...", FixmlMsg.DebugCategory);
			thread.Abort();
			thread = null;
		}

		// procedura wątku z obsługą kanału asynchronicznego
		private void ThreadProc()
		{
			try
			{
				using (Socket socket = GetAsyncSocket())
				{
					while (socket.Connected)
					{
						Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "");
						FixmlMsg msg;
						// odbiór komunikatu, na razie w "bezimiennej" klasie bazowej FixmlMsg
						try { msg = new FixmlMsg(socket); }
						catch (ThreadAbortException) { throw; }
						catch (FixmlSocketException) { throw; }
						catch (Exception e) { e.PrintError(); continue; }
						// rozpoznanie typu komunikatu i przetworzenie na konkretną klasę pochodną z FixmlMsg
						try { msg = FixmlMsg.GetParsedMsg(msg); }
						catch (ThreadAbortException) { throw; }
						catch (Exception e) { e.PrintError(); }
						// wywołanie podpiętych metod obsługi danego komunikatu
						try
						{
							if (AsyncMessageEvent != null) AsyncMessageEvent(msg);
							if (msg is AppMessageReportMsg) 
								if (AppReportMsgEvent != null) AppReportMsgEvent((AppMessageReportMsg)msg);
							if (msg is ExecutionReportMsg)
								if (ExecReportMsgEvent != null) ExecReportMsgEvent((ExecutionReportMsg)msg);
							if (msg is MarketDataIncRefreshMsg)
								if (MarketDataMsgEvent != null) MarketDataMsgEvent((MarketDataIncRefreshMsg)msg);
							if (msg is TradingSessionStatusMsg)
								if (SessionStatusMsgEvent != null) SessionStatusMsgEvent((TradingSessionStatusMsg)msg);
							if (msg is NewsMsg)
								if (NewsMsgEvent != null) NewsMsgEvent((NewsMsg)msg);
							if (msg is StatementMsg)
								if (StatementMsgEvent != null) StatementMsgEvent((StatementMsg)msg);
							if (msg is UserResponseMsg)
								if (UserResponseMsgEvent != null) UserResponseMsgEvent((UserResponseMsg)msg);
						}
						catch (ThreadAbortException) { throw; }
						catch (Exception e) { e.PrintError(); }
					}
				}
			}
			catch (ThreadAbortException) { Thread.ResetAbort(); }
			catch (Exception e) { e.PrintError(); }
			Debug.WriteLineIf(FixmlMsg.DebugInternals.Enabled, "NolClient.ThreadProc stop", FixmlMsg.DebugCategory);
		}

		#endregion



		#region Events

		/// <summary>
		/// Zdarzenie odbioru dowolnego komunikatu w kanale asynchronicznym
		/// </summary>
		public event Action<FixmlMsg> AsyncMessageEvent;

		/// <summary>
		/// Zdarzenie odbioru komunikatu "ApplMsgRpt"
		/// </summary>
		public event Action<AppMessageReportMsg> AppReportMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru komunikatu "ExecRpt"
		/// </summary>
		public event Action<ExecutionReportMsg> ExecReportMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru komunikatu "MktDataInc"
		/// </summary>
		public event Action<MarketDataIncRefreshMsg> MarketDataMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru komunikatu "TrdgSesStat"
		/// </summary>
		public event Action<TradingSessionStatusMsg> SessionStatusMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru komunikatu "News"
		/// </summary>
		public event Action<NewsMsg> NewsMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru komunikatu "Statement"
		/// </summary>
		public event Action<StatementMsg> StatementMsgEvent;
		/// <summary>
		/// Zdarzenie odbioru asynchronicznego(!) komunikatu "UserRsp" 
		/// (np. info o utracie połączenia, nie dotyczy normalnej odpowiedzi po UserRequestMsg)
		/// </summary>
		public event Action<UserResponseMsg> UserResponseMsgEvent;

		/// <summary>
		/// Zdarzenie (IBosClient) informujące o aktualizacji danych rachunku. 
		/// </summary>
		public event Action<AccountData> AccountUpdateEvent;
		/// <summary>
		/// Zdarzenie (IBosClient) informujące o aktualizacji informacji o zleceniu na rachunku.
		/// </summary>
		public event Action<OrderData> OrderUpdateEvent;
		/// <summary>
		/// Zdarzenie (IBosClient) informujące o aktualizacji stanu notowań rynkowych.
		/// </summary>
		public event Action<MarketData> MarketUpdateEvent;

		#endregion



		#region User login, logout

		/// <summary>
		/// Próba zalogowania się w aplikacji NOL3. 
		/// Normalnie metoda ta jest wywoływana automatycznie już przy utworzeniu obiektu NolClient...
		/// chyba że skorzystaliśmy z jednego z dodatkowych konstruktorów, pomijając automatyczne zalogowanie.
		/// </summary>
		public void Login()
		{
			Debug.WriteLine("\nLogin...");
		StartLogin:
			using (Socket socket = GetSyncSocket())
			{
				UserRequestMsg request = new UserRequestMsg();
				request.Type = UserRequestType.Login;
				request.Username = "BOS";
				request.Password = "BOS";
				request.Send(socket);
				try
				{
					UserResponseMsg response = new UserResponseMsg(socket);
					if ((response.Status == UserStatus.Other) && (response.StatusText == "User is already logged"))
						MyUtil.PrintWarning("NOL says: We're already logged in !?");
					else
						if (response.Status != UserStatus.LoggedIn)
							throw new FixmlErrorMsgException(response);
				}
				catch (BizMessageRejectException e)
				{
					// całe to przechwytywanie wyjątków i powtórki możnaby pominąć, gdyby NOL nie blokował 
					// numerku ReqID, jeśli jego poprzedni klient nie zrealizował prawidłowo logowania/wylogowania
					if (e.Msg.RejectText == "Incorrect UserReqID")
						if (request.Id < 100) goto StartLogin;  // każdy kolejny UserRequestMsg z większym "Id"
						else throw new FixmlException("UserReqID limit reached!");
					else throw;
				}
			}
			loggedIn = true;
			Debug.WriteLine("Login OK\n");
		}

		/// <summary>
		/// Próba wylogowania się z aplikacji NOL3. 
		/// Normalnie metoda ta jest wywoływana automatycznie przy zwalnianiu obiektu NolClient...
		/// ale nic nie szkodzi wywołać ją wcześniej jawnie (poza tym, że połączenie przestanie działać).
		/// </summary>
		public void Logout()
		{
			Debug.WriteLine("\nLogout...");
			loggedIn = false;  // <- nawet, jeśli niżej będzie błąd (żeby się nie powtarzał w destruktorze)
			statusOn = false;  // resetujemy też zmienne informujące o włączonym stanie subskrypcji
			mdReqId = null;    // (na wypadek ponownego połączenia - żeby mógł wtedy wznowić subkrypcję)
			using (Socket socket = GetSyncSocket())
			{
				UserRequestMsg request = new UserRequestMsg();
				request.Type = UserRequestType.Logout;
				request.Username = "BOS";
				request.Send(socket);
				UserResponseMsg response = new UserResponseMsg(socket);
				if (response.Status != UserStatus.LoggedOut)
					throw new FixmlErrorMsgException(response);
			}
			Debug.WriteLine("Logout OK\n");
		}

		// wewnętrzna obsługa asynchronicznego komunikatu "UserRsp"
		private void AsyncUserResponseMsgHandler(UserResponseMsg msg)
		{
			// (msg.Status == UserStatus.Other)
			// msg.StatusText:
			// 1 - zamkniecie aplikacji NOL3 
			// 2 - NOL3 jest offline 
			// 3 - NOL3 jest online 
			// 4 - aplikacja NOL3 nie jest uruchomiona
			Trace.WriteLine(" connection terminated ?! ");
			ThreadStop();
		}

		#endregion


		#region MarketData subscription

		private uint? mdReqId = null;
		private HashSet<MDEntryType> mdEntryTypes = new HashSet<MDEntryType>();
		private HashSet<FixmlInstrument> mdInstruments = new HashSet<FixmlInstrument>();
		private Dictionary<FixmlInstrument, MDResults> mdResults = new Dictionary<FixmlInstrument, MDResults>();

		/// <summary>
		/// Aktywacja odbioru (subskrypcji) informacji o bieżących notowaniach giełdowych.
		/// Treść tych informacji ustalamy (najlepiej wcześniej) korzystając z metod "MarketDataSubscription*".
		/// </summary>
		public void MarketDataStart()
		{
			if (mdReqId != null) MarketDataStop();
			lock (mdResults)
			{
				mdResults.Clear();
				Debug.WriteLine("\nMarketDataStart...");
				using (Socket socket = GetSyncSocket())
				{
					MarketDataRequestMsg request = new MarketDataRequestMsg();
					request.Type = SubscriptionRequestType.StartSubscription;
					request.MarketDepth = 1;
					request.EntryTypes = mdEntryTypes.ToArray();
					request.Instruments = mdInstruments.ToArray();
					request.Send(socket);
					MarketDataFullRefreshMsg response = new MarketDataFullRefreshMsg(socket);
					if (response.RequestId != request.Id)
						throw new FixmlException("Unexpected MktDataFull ReqID.");
					mdReqId = request.Id;
				}
				Debug.WriteLine("MarketDataStart OK\n");
			}
		}

		/// <summary>
		/// Deaktywacja odbioru (subskrypcji) informacji o bieżących notowaniach giełdowych.
		/// To, co się udało do tej pory zebrać, pozostaje nadal w pamięci - dostępne przez "MarketDataResults".
		/// </summary>
		public void MarketDataStop()
		{
			if (mdReqId == null) return;
			lock (mdResults)
			{
				Debug.WriteLine("\nMarketDataStop...");
				using (Socket socket = GetSyncSocket())
				{
					MarketDataRequestMsg request = new MarketDataRequestMsg();
					request.Type = SubscriptionRequestType.CancelSubscription;
					request.Send(socket);
					MarketDataFullRefreshMsg response = new MarketDataFullRefreshMsg(socket);
					if (response.RequestId != request.Id)
						throw new FixmlException("Unexpected MktDataFull ReqID.");
				}
				mdReqId = null;
				Debug.WriteLine("MarketDataStop OK\n");
			}
		}

		// wewnętrzna obsługa komunikatu "MktDataInc"
		// - dopisuje nowe informacje do struktury "mdResults"
		private void MarketDataMsgHandler(MarketDataIncRefreshMsg msg)
		{
			// blokada/synchronizacja tutaj głównie po to, by opóźnić obsługę tego komunikatu 
			// aż zakończymy metodę MarketDataStart(), gdzie m.in. dopiero jest ustawiany nowy mdReqId
			// (asynchroniczne MktDataInc zaczynają tu przychodzić jeszcze zanim tam odbierzemy MktDataFull)
			lock (mdResults)
			{
				// bezpośrednio po zmianie subskrypcji (co jest w innym wątku) może nam tu jeszcze dotrzeć
				// komunikat z poprzednim Id - ignorujemy, by nie powstały duplikaty w historii transakcji
				if (msg.RequestId != mdReqId) return;

				MarketData data;
				if (MarketUpdateEvent != null)
					foreach (MDEntry entry in msg.Entries)
						if (MarketData_GetData(entry, out data)) MarketUpdateEvent(data);

				// TODO: MDResults zastąpić nowymi klasami DTO(?) albo całkiem wyrzucić...
				// a na razie pomijamy, jeśli włączono "MarketUpdates" - żeby nie dublować danych w pamięci
				if (MarketUpdateEvent == null)
				foreach (MDEntry entry in msg.Entries)
					MarketDataResults(entry.Instrument).AddEntry(entry);
			}
		}

		/// <summary>
		/// Odczyt wszystkich zebranych informacji o bieżących notowaniach giełdowych.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<FixmlInstrument, MDResults>> MarketDataResults()
		{
			return mdResults;
		}

		/// <summary>
		/// Odczyt zebranych informacji o bieżących notowaniach wskazanego instrumentu.
		/// </summary>
		/// <param name="instrument"></param>
		/// <returns></returns>
		public MDResults MarketDataResults(FixmlInstrument instrument)
		{
			if (!mdResults.ContainsKey(instrument))
				mdResults.Add(instrument, new MDResults());
			return mdResults[instrument];
		}

		/// <summary>
		/// Odczyt zebranych informacji o bieżących notowaniach wskazanego instrumentu (po symbolu).
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public MDResults MarketDataResults(string symbol)
		{
			return MarketDataResults(FixmlInstrument.FindBySym(symbol));
		}

		// wewnętrzna metoda aktualizująca bieżący zakres subskrybowanych informacji
		private void MarketDataSubscriptionChange(IEnumerable<MDEntryType> types, IEnumerable<FixmlInstrument> instr)
		{
			bool wasActive = (mdReqId != null);
			if (wasActive) MarketDataStop();
			mdEntryTypes = new HashSet<MDEntryType>(types);
			mdInstruments = new HashSet<FixmlInstrument>(instr);
			if (wasActive && types.Any() && instr.Any()) MarketDataStart();
		}

		/// <summary>
		/// Całkowite wyczyszczenie zakresu subskrypcji. 
		/// Po tej operacji wszystko (typ informacji oraz listę instrumentów) ustawiamy od nowa.
		/// </summary>
		public void MarketDataSubscriptionClear()
		{
			MarketDataStop();
			mdEntryTypes.Clear();
			mdInstruments.Clear();
		}

		/// <summary>
		/// Dodanie do subskrypcji wskazanych typów rekordów.
		/// </summary>
		/// <param name="types"></param>
		public void MarketDataSubscriptionAdd(params MDEntryType[] types)
		{
			MarketDataSubscriptionChange(mdEntryTypes.Union(types), mdInstruments);
		}

		/// <summary>
		/// Usunięcie z subskrypcji wskazanych typów rekordów.
		/// </summary>
		/// <param name="types"></param>
		public void MarketDataSubscriptionRemove(params MDEntryType[] types)
		{
			MarketDataSubscriptionChange(mdEntryTypes.Except(types), mdInstruments);
		}

		/// <summary>
		/// Dodanie do subskrypcji wskazanych instrumentów.
		/// </summary>
		/// <param name="instruments"></param>
		public void MarketDataSubscriptionAdd(params FixmlInstrument[] instruments)
		{
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Union(instruments));
		}

		/// <summary>
		/// Usunięcie z subskrypcji wskazanych instrumentów.
		/// </summary>
		/// <param name="instruments"></param>
		public void MarketDataSubscriptionRemove(params FixmlInstrument[] instruments)
		{
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Except(instruments));
		}

		/// <summary>
		/// Dodanie do subskrypcji wskazanych instrumentów (po ich symbolach). 
		/// </summary>
		/// <param name="symbols"></param>
		public void MarketDataSubscriptionAdd(params string[] symbols)
		{
			IEnumerable<FixmlInstrument> instruments = symbols.Select(sym => FixmlInstrument.FindBySym(sym));
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Union(instruments));
		}

		/// <summary>
		/// Usunięcie z subskrypcji wskazanych instrumentów (po ich symbolach).
		/// </summary>
		/// <param name="symbols"></param>
		public void MarketDataSubscriptionRemove(params string[] symbols)
		{
			IEnumerable<FixmlInstrument> instruments = symbols.Select(sym => FixmlInstrument.FindBySym(sym));
			MarketDataSubscriptionChange(mdEntryTypes, mdInstruments.Except(instruments));
		}

		#endregion


		#region TradingSessionStatus subscription

		private bool statusOn = false;

		/// <summary>
		/// Aktywacja odbioru informacji o statusie sesji (komunikat "TrdgSesStat": 
		/// informuje o zmianie fazy sesji, równoważeniu subskrybowanych instrumentów itp.).
		/// Aby na te zmiany reagować, należy się podłączyć pod zdarzenie "SessionStatusMsgEvent".
		/// </summary>
		public void TradingSessionStatusStart()
		{
			if (statusOn) return;
			Debug.WriteLine("\nTradingSessionStatusStart...");
			using (Socket socket = GetSyncSocket())
			{
				TrdSesStatusRequestMsg request = new TrdSesStatusRequestMsg();
				request.Type = SubscriptionRequestType.StartSubscription;
				request.Send(socket);
				TradingSessionStatusMsg response = new TradingSessionStatusMsg(socket);
				if (response.RequestId != request.Id)
					throw new FixmlException("Unexpected TrdgSesStat ReqID.");
				// mamy taki status - ale czy to tego requesta dotyczy, to ja tylko zgadywać mogę... :/
				if (response.SessionStatus == TradingSessionStatus.RequestRejected)
					throw new FixmlException("TrdgSesStat rejected, reason = " + response.RejectReason);
			}
			Debug.WriteLine("TradingSessionStatusStart OK\n");
			statusOn = true;
		}

		/// <summary>
		/// Deaktywacja odbioru informacji o statusie sesji.
		/// </summary>
		public void TradingSessionStatusStop()
		{
			if (!statusOn) return;
			Debug.WriteLine("\nTradingSessionStatusStop...");
			using (Socket socket = GetSyncSocket())
			{
				TrdSesStatusRequestMsg request = new TrdSesStatusRequestMsg();
				request.Type = SubscriptionRequestType.CancelSubscription;
				request.Send(socket);
				TradingSessionStatusMsg response = new TradingSessionStatusMsg(socket);
				if (response.RequestId != request.Id)
					throw new FixmlException("Unexpected TrdgSesStat ReqID.");
				// mamy taki status - ale czy to tego requesta dotyczy, to ja tylko zgadywać mogę... :/
				if (response.SessionStatus == TradingSessionStatus.RequestRejected)
					throw new FixmlException("TrdgSesStat rejected, reason = " + response.RejectReason);
			}
			Debug.WriteLine("TradingSessionStatusStop OK\n");
			statusOn = false;
		}

		#endregion


		#region MarketUpdates subscription

		// metoda IBosClient do ustawiania "filtra" subskrybowanych notowań.
		public void MarketUpdatesSubscription(Instrument[] instruments)
		{
			MarketDataSubscriptionClear();
			if (instruments != null && instruments.Length > 0)
			{
				var fixmlInstruments = instruments.Select(i => FixmlInstrument.Find(i)).ToArray();
				MarketDataSubscriptionAdd(fixmlInstruments);
				MarketDataSubscriptionAdd(MDEntryTypes.BasicBook);
				MarketDataSubscriptionAdd(MDEntryType.Trade);
				MarketDataStart();
				TradingSessionStatusStart();
			}
			else TradingSessionStatusStop();
		}

		// funkcja pomocnicza konwertująca obiekt Fixml.MDEntry na DTO.MarketData
		private static bool MarketData_GetData(MDEntry entry, out MarketData data)
		{
			data = new MarketData();
			data.Instrument = entry.Instrument.Convert();
			switch (entry.EntryType)
			{
				case MDEntryType.Buy: data.BuyOffer = MarketData_GetOfferData(entry); break;
				case MDEntryType.Sell: data.SellOffer = MarketData_GetOfferData(entry); break;
				case MDEntryType.Trade: data.Trade = MarketData_GetTradeData(entry); break;
				default: return false;   // pozostałe na razie pomijamy
			}
			return true;
		}

		// funkcja pomocnicza konwertująca dane z Fixml.MDEntry na DTO.MarketOfferData
		private static MarketOfferData MarketData_GetOfferData(MDEntry entry)
		{
			var offer = new MarketOfferData();
			offer.Level = (int)entry.Level;
			offer.Update = (entry.UpdateAction != MDUpdateAction.New);
			if (entry.UpdateAction != MDUpdateAction.Delete)
			{
				switch (entry.PriceStr)
				{
					case "PKC": offer.PriceType = PriceType.PKC; break;
					case "PCR": offer.PriceType = PriceType.PCR; break;
					case "PCRO": offer.PriceType = PriceType.PCRO; break;
				}
				offer.PriceLimit = entry.Price;
				offer.Volume = (uint)entry.Size;
				offer.Count = (uint)entry.Orders;
			}
			return offer;
		}

		// funkcja pomocnicza konwertująca dane z Fixml.MDEntry na DTO.MarketTradeData
		private static MarketTradeData MarketData_GetTradeData(MDEntry entry)
		{
			var trade = new MarketTradeData();
			trade.Time = entry.DateTime;
			trade.Price = (decimal)entry.Price;
			trade.Quantity = (uint)entry.Size;
			return trade;
		}

		#endregion


		#region Orders

		// Metoda IBosClient do składania nowego zlecenia.
		public string OrderCreate(OrderData data)
		{
			string clientId;
			Debug.WriteLine("\nOrderCreate...");
			using (Socket socket = NolClient.GetSyncSocket())
			{
				NewOrderSingleMsg request = new NewOrderSingleMsg();
				clientId = request.ClientOrderId;  // automatycznie przydzielone kolejne Id
				request.Account = data.AccountNumber;
				request.CreateTime = data.MainData.CreateTime;
				request.Instrument = FixmlInstrument.Find(data.MainData.Instrument);
				request.Side = (data.MainData.Side == BosOrderSide.Buy) ? OrderSide.Buy : OrderSide.Sell;
				request.Type = Order_GetType(data.MainData);
				request.Price = data.MainData.PriceLimit;
				request.StopPrice = data.MainData.ActivationPrice;
				request.Quantity = data.MainData.Quantity;
				request.MinimumQuantity = data.MainData.MinimumQuantity;
				request.DisplayQuantity = data.MainData.VisibleQuantity;
				request.TimeInForce = Order_GetTimeInForce(data.MainData);
				request.ExpireDate = data.MainData.ExpirationDate;
				request.Send(socket);
				ExecutionReportMsg response = new ExecutionReportMsg(socket);
			}
			Debug.WriteLine("OrderCreate OK\n");
			return clientId;
		}

		// Metoda IBosClient do modyfikacji istniejącego zlecenia.
		public void OrderReplace(OrderData data)
		{
			Debug.WriteLine("\nOrderReplace...");
			using (Socket socket = NolClient.GetSyncSocket())
			{
				OrderReplaceRequestMsg request = new OrderReplaceRequestMsg();
				request.Account = data.AccountNumber;
				request.BrokerOrderId2 = data.BrokerId;
				request.Instrument = FixmlInstrument.Find(data.MainData.Instrument);
				request.Side = (data.MainData.Side == BosOrderSide.Buy) ? OrderSide.Buy : OrderSide.Sell;
				request.Type = Order_GetType(data.MainData);
				request.Price = data.MainData.PriceLimit;
				request.StopPrice = data.MainData.ActivationPrice;
				request.Quantity = data.MainData.Quantity;
				request.MinimumQuantity = data.MainData.MinimumQuantity;
				request.DisplayQuantity = data.MainData.VisibleQuantity;
				request.TimeInForce = Order_GetTimeInForce(data.MainData);
				request.ExpireDate = data.MainData.ExpirationDate;
				request.Send(socket);
				ExecutionReportMsg response = new ExecutionReportMsg(socket);
			}
			Debug.WriteLine("OrderReplace OK\n");
		}

		// Metoda IBosClient do anulowania istniejącego zlecenia.
		public void OrderCancel(OrderData data)
		{
			Debug.WriteLine("\nOrderCancel...");
			using (Socket socket = NolClient.GetSyncSocket())
			{
				OrderCancelRequestMsg request = new OrderCancelRequestMsg();
				request.Account = data.AccountNumber;
				request.BrokerOrderId2 = data.BrokerId;
				request.Instrument = FixmlInstrument.Find(data.MainData.Instrument);
				request.Side = (data.MainData.Side == BosOrderSide.Buy) ? OrderSide.Buy : OrderSide.Sell;
				request.Quantity = data.MainData.Quantity;
				request.Send(socket);
				ExecutionReportMsg response = new ExecutionReportMsg(socket);
			}
			Debug.WriteLine("OrderCancel OK\n");
		}

		// funkcja pomocnicza przygotowująca typ zlecenia FIXML
		private static OrderType Order_GetType(OrderMainData data)
		{
			switch (data.PriceType)
			{
				case PriceType.Limit: return (data.ActivationPrice != null) ? OrderType.StopLimit : OrderType.Limit;
				case PriceType.PKC: return (data.ActivationPrice != null) ? OrderType.StopLoss : OrderType.PKC;
				default: return OrderType.PCR_PCRO;
			}
		}

		// funkcja pomocnicza przygotowująca parametr FIXML TimeInForce 
		private static OrdTimeInForce Order_GetTimeInForce(OrderMainData data)
		{
			if (data.ImmediateOrCancel) return OrdTimeInForce.WiA;
			if (data.MinimumQuantity == data.Quantity) return OrdTimeInForce.WuA;
			if (data.PriceType == PriceType.PCRO)
				return (DateTime.Now.Hour < 12) ? OrdTimeInForce.Opening : OrdTimeInForce.Closing;
			if (data.ExpirationDate != null) return OrdTimeInForce.Date;
			return OrdTimeInForce.Day;
		}


		// wewnętrzna obsługa komunikatu "ExecRpt"
		// - wywołuje zdarzenie "OrderUpdateEvent"
		private void ExecReportMsgHandler(ExecutionReportMsg msg)
		{
			if (OrderUpdateEvent != null)
			{
				var order = new OrderData();
				order.AccountNumber = msg.Account;
				order.BrokerId = msg.BrokerOrderId2;
				order.ClientId = msg.ClientOrderId;

				// UWAGA: odczyt danych z komunikatu ExecRpt odbywa się "na czuja"... bo dostarczoną 
				// z Bossy/Comarchu dokumentacją na ten temat to można sobie najwyżej w kominku podpalić.
				// Generalnie wszystko jest rozjechane, ale te statusy zleceń to już chyba po pijaku pisali.
				//   Tyle - musiałem to tu napisać !!!  ;-P
				// Bo mnie już krew zalewa, jak widzę co ten NOL3 wysyła i gdzie (w których polach)... 
				// I że w ogóle musiałem te wszystkie możliwe przypadki samodzielnie analizować...
				// A jak za miesiąc przestanie działać, bo coś tam "naprawią", to chyba kogoś postrzelę ;-(


				// raport o wykonaniu - być może cząstkowym - danego zlecenia
				// (z praktyki wynika, że zawsze wcześniej dostaniemy "pełen" ExecRpt z pozostałymi
				// informacjami o tym zleceniu... dlatego teraz odbieramy sobie tylko raport z tej transakcji)
				if (msg.ExecType == ExecReportType.Trade)
				{
					order.TradeReport = new OrderTradeData();
					order.TradeReport.Time = (DateTime)msg.TransactionTime;
					order.TradeReport.Price = (decimal)msg.Price;  // LastPrice !?
					order.TradeReport.Quantity = (uint)msg.Quantity;  // LastQuantity !?
					order.TradeReport.NetValue = (decimal)msg.NetMoney;
					order.TradeReport.Commission = (decimal)msg.CommissionValue;
				}
				else
				{
					// w pozostałych przypadkach wygląda na to, że lepiej się oprzeć na polu "Status"
					// (bo ExecType czasem jest, czasem nie ma - różnie to z nim bywa... a Status jest chyba zawsze)
					order.StatusReport = new OrderStatusData();
					order.StatusReport.Status = ExecReport_GetStatus(msg);
					order.StatusReport.Quantity = (uint)msg.CumulatedQuantity;
					order.StatusReport.NetValue = (decimal)msg.NetMoney;
					order.StatusReport.Commission = (decimal)msg.CommissionValue;  // czasem == 0, ale dlaczego!? kto to wie... 

					// pozostałe dane - żeby się nie rozdrabniać - też aktualizujemy za każdym razem
					// (teoretycznie wystarczyłoby przy "new" i "replace"... ale czasem jako pierwsze
					// przychodzi np. "filled" i kto wie co jeszcze innego, więc tak będzie bezpieczniej)
					order.MainData = new OrderMainData();
					order.MainData.CreateTime = (DateTime)msg.TransactionTime;
					order.MainData.Instrument = msg.Instrument.Convert();
					order.MainData.Side = (msg.Side == Fixml.OrderSide.Buy) ? BosOrderSide.Buy : BosOrderSide.Sell;
					order.MainData.PriceType = ExecReport_GetPriceType(msg);
					if (order.MainData.PriceType == PriceType.Limit)
						order.MainData.PriceLimit = msg.Price;
					if ((msg.Type == OrderType.StopLimit) || (msg.Type == OrderType.StopLoss))
						order.MainData.ActivationPrice = msg.StopPrice;
					order.MainData.Quantity = (uint)msg.Quantity;
					order.MainData.MinimumQuantity = (msg.TimeInForce == OrdTimeInForce.WuA) ? msg.Quantity : msg.MinimumQuantity;
					order.MainData.VisibleQuantity = msg.DisplayQuantity;
					order.MainData.ImmediateOrCancel = (msg.TimeInForce == OrdTimeInForce.WiA);
					order.MainData.ExpirationDate = (msg.TimeInForce == OrdTimeInForce.Date) ? msg.ExpireDate : null;
				}

				// wywołanie zdarzenia z przygotowanymi danymi
				OrderUpdateEvent(order);
			}
		}

		// funkcja pomocnicza zamieniająca status zlecenia FIXML na nasz BosOrderStatus
		private static BosOrderStatus ExecReport_GetStatus(ExecutionReportMsg msg)
		{
			switch (msg.Status)
			{
				case ExecReportStatus.New: return BosOrderStatus.Active;
				case ExecReportStatus.PartiallyFilled: return BosOrderStatus.ActiveFilled;
				case ExecReportStatus.Canceled: return ((msg.CumulatedQuantity ?? 0) > 0) ? BosOrderStatus.CancelledFilled : BosOrderStatus.Cancelled;
				case ExecReportStatus.Filled: return BosOrderStatus.Filled;
				case ExecReportStatus.Expired: return BosOrderStatus.Expired;
				case ExecReportStatus.Rejected: return BosOrderStatus.Rejected;
				case ExecReportStatus.PendingReplace: return BosOrderStatus.PendingReplace;
				case ExecReportStatus.PendingCancel: return BosOrderStatus.PendingCancel;
				default: throw new ArgumentException("Unknown ExecReport-Status");
			}
		}

		// funkcja pomocnicza zamieniająca typ zlecenia FIXML na nasz DTO.PriceType
		private static PriceType ExecReport_GetPriceType(ExecutionReportMsg msg)
		{
			switch (msg.Type)
			{
				case OrderType.Limit:
				case OrderType.StopLimit: return PriceType.Limit;
				case OrderType.PKC:
				case OrderType.StopLoss: return PriceType.PKC;
				case OrderType.PCR_PCRO:
					var time = msg.TimeInForce;
					var pcro = ((time == OrdTimeInForce.Opening) || (time == OrdTimeInForce.Closing));
					return pcro ? PriceType.PCRO : PriceType.PCR;
				default: throw new ArgumentException("Unknown ExecReport-OrderType");
			}
		}

		#endregion


		#region Other events handling

		// wewnętrzna obsługa komunikatu "Statement"
		// - wywołuje zdarzenia "AccountUpdateEvent", po jednym na każdy rachunek
		private void StatementMsgHandler(StatementMsg msg)
		{
			if (AccountUpdateEvent != null)
			{
				foreach (var statement in msg.Statements)
				{
					var account = new AccountData();
					account.Number = statement.AccountNumber;
					// otwarte pozycje...
					account.Papers = statement.Positions.
						Select(p => new Paper {
							Instrument = p.Key.Convert(),
							Account110 = p.Value.Acc110,
							Account120 = p.Value.Acc120,
						}).ToArray();
					// najważniejsze kwoty...
					account.AvailableCash = statement.Funds[StatementFundType.Cash];
					if (!statement.Funds.ContainsKey(StatementFundType.Deposit))
					{
						// rachunek akcyjny
						account.AvailableFunds = statement.Funds[StatementFundType.CashReceivables];
					}
					else
					{
						// rachunek kontraktowy
						account.AvailableFunds = account.AvailableCash + statement.Funds[StatementFundType.DepositFree];
						if (statement.Funds.ContainsKey(StatementFundType.DepositDeficit))
							account.DepositDeficit = statement.Funds[StatementFundType.DepositDeficit];
						account.DepositBlocked = statement.Funds[StatementFundType.DepositBlocked];
						account.DepositValue = statement.Funds[StatementFundType.Deposit];
					}
					account.PortfolioValue = statement.Funds[StatementFundType.PortfolioValue];

					// wywołanie zdarzenia z przygotowanymi danymi
					AccountUpdateEvent(account);
				}
			}
		}

		#endregion

	}
}
