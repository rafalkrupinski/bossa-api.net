using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Xml;
using pjank.BossaAPI;
using pjank.BossaAPI.Fixml;
using System.Threading;

namespace pjank.BossaAPI.TestApp1
{
	class Program
	{

		// główna funkcja programu
		public static void Main(string[] args)
		{
			InitDebugConsole();
			Trace.WriteLine("Hello!  [" + DateTime.Now + "]\n");
			try
			{
				InitFixmlDebugOptions();  // <- można tak, można też w <configuration> aplikacji

				while (true)
					try
					{
						if (!SelectTestModule()) break;
					}
					catch (Exception e)
					{
						MyUtil.PrintError(e);
					}
			}
			finally
			{
				Trace.WriteLine("\nBye!  [" + DateTime.Now + "]\n");
				Console.ReadKey(true);
				CloseDebugConsole();
			}
		}



		// różne opcje wyświetlania treści przesyłanych komunikatów
		private static void InitFixmlDebugOptions()
		{
			//FixmlMsg.DebugInternals.Enabled = true;
			//FixmlMsg.DebugOriginalXml.Enabled = true;
			//FixmlMsg.DebugFormattedXml.Enabled = true;
			FixmlMsg.DebugParsedMessage.Enabled = true;
		}


		// nazwa pliku z zapisem całej sesji
		// (do tego parę wcześniejszych wersji tego pliku z doklejoną cyferką na końcu)
		private const string debugFileName = "debug.log";

		// inicjalizacja okienka oraz pliku loga
		private static void InitDebugConsole()
		{
			if (File.Exists(debugFileName + 2)) File.Delete(debugFileName + 2);
			if (File.Exists(debugFileName + 1)) File.Move(debugFileName + 1, debugFileName + 2);
			if (File.Exists(debugFileName)) File.Move(debugFileName, debugFileName + 1);
			Debug.Listeners.Add(new TextWriterTraceListener(debugFileName));
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			Console.BufferWidth = 1000;
			// (wskazana mała czcionka dla okna konsoli... ewentualnie wyłączyć poniższe)
			Console.WindowWidth = Math.Min(140, Console.LargestWindowWidth);
			Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
		}

		// zamknięcie pliku loga
		private static void CloseDebugConsole()
		{
			Debug.Flush();
			Debug.Close();
		}



		/// <summary>
		/// Wyświetla menu dostępnych do wyboru testów i odpala ten wybrany. 
		/// </summary>
		/// <returns>Zwraca false, jeśli naciśnięto klawisz Escape</returns>
		private static bool SelectTestModule()
		{
			Console.WriteLine("\n\nSelect test module:   (Esc - exit)");
			Console.WriteLine(" 1 - basic NolClient usage: simple Login and Logout");
			Console.WriteLine(" 2 - advanced NolClient usage, custom message handling");
			Console.WriteLine(" 3 - MarketData and SessionStatus subscription using NolClient");
			Console.WriteLine(" 4 - low-level Order Requests handling using FixmlMsg subclasses");
			ConsoleKey key = Console.ReadKey(true).Key;
			Trace.WriteLine("\nMenu selection: " + key + "\n");
			switch (key)
			{
				case ConsoleKey.D1: TestModule1(); break;
				case ConsoleKey.D2: TestModule2(); break;
				case ConsoleKey.D3: TestModule3(); break;
				case ConsoleKey.D4: TestModule4(); break;
				case ConsoleKey.Escape: return false;
			}
			return true;
		}


		/// <summary>
		/// Proste zalogowanie i wylogowanie użytkownika z użyciem klasy "NolClient".
		/// Przykład podpięcia zdarzenia pod wybrany rodzaj komunikatu asynchronicznego.
		/// </summary>
		private static void TestModule1()
		{
			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (NolClient nol = new NolClient())
			{
				// podpinamy się pod konkretny rodzaj komunikatów, tutaj - wyciąg z rachunku
				nol.StatementMsgEvent += new Action<StatementMsg>(nol_StatementMsgHandler);

				// po zalogowaniu NOL3 od razu przysyła info o złożonych zleceniach, komunikaty z wizjera itp.
				Console.WriteLine("\n... async connection thread working in background ... \n");
				Thread.Sleep(2000);

				// czekamy na kolejne komunikaty, jakie mogą nadchodzić w trakcie sesji...
				Console.WriteLine("\nPress any key... to exit   \n\n");
				Console.ReadKey(true);
				Console.WriteLine("\n\nThank you :)\n");

			}  // tu następuje wylogowanie
		}

		/// <summary>
		/// Przykład obsługi odebranego w klasie NolClient (TestModule1) komunikatu asynchronicznego.
		/// </summary>
		/// <param name="msg">Tutaj - obiekt komunikatu FIXML typu Statement</param>
		static void nol_StatementMsgHandler(StatementMsg msg)
		{
			decimal cash = 0;
			foreach (var statement in msg.Statements)
				if (statement.Funds.ContainsKey(StatementFundType.Cash))
					cash += statement.Funds[StatementFundType.Cash];
			Console.WriteLine("\nCash summary from all accounts = " + cash + "\n");
		}


		/// <summary>
		/// Bardziej zaawansowany przykład użycia "NolClient": zalogowanie dopiero na żądanie, 
		/// bez uruchamiania wątku odbierającego komunikaty asynchroniczne (można go obsłużyć samemu).
		/// Samodzielne przygotowanie, wysyłka i odbiór przykładowego message'a.
		/// </summary>
		private static void TestModule2()
		{
			using (NolClient nol = new NolClient(false, false))
			{
				// zalogowanie użytkownika
				Console.WriteLine("\nPress any key... to log in   [Esc - cancel]\n");
				if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;
				nol.Login();

				// otwarcie kanału asynchronicznego
				// (na razie nic tu z niego nie odbieramy, bo do tego przydałby się oddzielny wątek)
				Console.WriteLine("\nPress any key... to open async socket   [Esc - skip]\n");
				Socket asyncSocket = null;
				if (Console.ReadKey(true).Key != ConsoleKey.Escape)
					asyncSocket = NolClient.GetAsyncSocket();

				// wysyłka przykładowego komunikatu
				// (można skorzystać z gotowych klas zdefiniowanych w pjank.BossaAPI.Fixml, 
				// ale można też spreparować coś zupełnie własnego w oparciu o klasę CustomMsg)
				Console.WriteLine("\nPress any key... to send a custom message   [Esc - cancel]\n");
				if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;
				bool tmp = FixmlMsg.DebugOriginalXml.Enabled;
				try
				{
					FixmlMsg.DebugOriginalXml.Enabled = true;

					// otwarcie nowego połączenia (kanał synchroniczny za każdym razem nowy!)
					using (Socket syncSocket = NolClient.GetSyncSocket())
					{
						// przygotowanie komunikatu
						FixmlMsg request = new UserRequestMsg() {
							Username = "BOS",
							Type = UserRequestType.GetStatus,
						};
						// wysyłka komunikatu
						request.Send(syncSocket);

						// odbiór odpowiedzi
						Console.WriteLine("\nPress any key... to read the response\n");
						Console.ReadKey(true);
						FixmlMsg response = new FixmlMsg(syncSocket);
						Trace.WriteLine("\nResponse XML:\n" + response.Xml.FormattedXml() + "\n");

						// dokładniejsza analiza odpowiedzi (w klasie konkretnego rodzaju komunikatu)
						Console.WriteLine("Press any key... to parse the response message\n");
						Console.ReadKey(true);
						UserResponseMsg parsedResponse = new UserResponseMsg(response);
						Trace.WriteLine(String.Format("\nResponse parsed:\n Status = {0}, StatusText = '{1}'\n",
							parsedResponse.Status, parsedResponse.StatusText));
					}

					Console.WriteLine("\nPress any key... to send another custom message   [Esc - cancel]\n");
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					// otwarcie nowego połączenia (kanał synchroniczny za każdym razem nowy!)
					using (Socket syncSocket = NolClient.GetSyncSocket())
					{
						// tak można spreparować dowolny komunikat, również taki jeszcze nieistniejący ;->
						CustomMsg request = new CustomMsg("MyCustomRequest");
						XmlElement xmlElement = request.AddElement("Test");
						xmlElement.SetAttribute("attr1", "1");
						xmlElement.SetAttribute("attr2", "2");
						// wysyłka tak samodzielnie spreparowanego komunikatu
						request.Send(syncSocket);
						// odbiór odpowiedzi - tutaj powinniśmy otrzymać błąd... "BizMessageRejectException"
						// niestety aktualna wersja NOL3 zwraca nieprawidłowy XML, którego nie da się parsować
						Console.WriteLine("\nPress any key... to read the response\n");
						Console.ReadKey(true);
						FixmlMsg response = new FixmlMsg(syncSocket);
					}
				}
				catch (Exception e)
				{
					MyUtil.PrintError(e);
				}
				FixmlMsg.DebugOriginalXml.Enabled = tmp;
				Console.ReadKey(true);
				if (asyncSocket != null) asyncSocket.Close();

			}  // tu następuje automatyczne wylogowanie
		}


		/// <summary>
		/// Wykorzystanie klasy NolClient do subskrypcji bieżących notowań wybranych papierów wartościowych.
		/// </summary>
		private static void TestModule3()
		{
			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (NolClient nol = new NolClient())
			{
				try
				{
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					Trace.WriteLine("\n--- market data subscription start ---");

					// definiujemy filtr subskrybowanych danych 
					nol.MarketDataSubscriptionAdd(MDEntryTypes.BasicBook);
					nol.MarketDataSubscriptionAdd(MDEntryTypes.BasicTrade);
					FixmlInstrument fw20 = FixmlInstrument.FindBySym("FW20H12");
					nol.MarketDataSubscriptionAdd(fw20);

					// tutaj rozpoczynamy odbieranie notowań
					nol.MarketDataStart();

					// subskrypcja informacji o statusie sesji
					nol.TradingSessionStatusStart();

					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;


					Trace.WriteLine("\n--- market data subscription change ---");

					// modyfikujemy filtr, wyłączając na ten czas notowania
					// (inaczej resetowałby je po każdej kolejnej zmianie, w tym przykładzie 2-krotnie)
					nol.MarketDataStop();
					nol.MarketDataSubscriptionClear();
					nol.MarketDataSubscriptionAdd(MDEntryTypes.All);
					nol.MarketDataSubscriptionAdd("WIG20", "FW20M12", "FW20H12");
					nol.MarketDataStart();

					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;


					Trace.WriteLine("\n--- market data subscription stop ---");

					// zerujemy filtr (wyłączamy sybskrypcję notowań)
					nol.MarketDataSubscriptionClear();

					// wydruk wszystkich zgromadzonych notowań
					foreach (var x in nol.MarketDataResults())
						Debug.WriteLine(string.Format("{0,-9}  {1}\n", x.Key, x.Value));

					// wydruk wybranych danych konkretnego instrumentu
					var fw20data = nol.MarketDataResults(fw20);
					Debug.WriteLine(string.Format("{0} = {1,7}   LOP = {2}", fw20, fw20data.CurrentPrice, fw20data.Lop));

					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					// wyłączamy informacje o statusie sesji
					nol.TradingSessionStatusStop();

					// wciąż aktywny kanał asynchroniczny...
					Console.ReadKey(true);

					Trace.WriteLine("\n--- done ---");
				}
				catch (Exception e)
				{
					MyUtil.PrintError(e);
				}
			}  // tu następuje wylogowanie
		}


		/// <summary>
		/// Przykład wysyłki zleceń, korzystając bezpośrednio z klas NewOrderSingleMsg i spółki... 
		/// (póki co, jest to jedyna dostępna metoda - docelowo będzie do tego jeszcze prostszy interfejs).
		/// Test ten wrzuca na giełdę zlecenie kupna 1 x FW20 po 1000zł (*raczej* nie ma szans się zrealizować :)),
		/// następnie je modyfikuje ustawiając limit ceny oczko wyżej... aż ostatecznie całe zlecenie anuluje. 
		/// </summary>
		private static void TestModule4()
		{
			string accountNumber = "00-22-...";  // <- wpisz tu swój numer, żeby program nie musiał o niego pytać
			if (accountNumber.EndsWith("..."))
			{
				Console.Write("Podaj numer rachunku (końcówkę z " + accountNumber + "): ");
				string str = Console.ReadLine();
				accountNumber = accountNumber.Replace("...", str);
				Trace.WriteLine("Wybrany rachunek: " + accountNumber);
			}

			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (NolClient nol = new NolClient())
			{
				Thread.Sleep(2000);
				bool tmp = FixmlMsg.DebugFormattedXml.Enabled;
				try
				{
					ExecutionReportMsg execReport;

					// --- wysyłka nowego zlecenia --- 
					Console.WriteLine("\nPress any key... to send NEW order request    [Esc - exit]\n");
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					NewOrderSingleMsg newRequest = new NewOrderSingleMsg();
					newRequest.Account = accountNumber;
					newRequest.Side = OrderSide.Buy;
					newRequest.Instrument = FixmlInstrument.FindBySym("FW20H12");
					newRequest.Quantity = 1;
					newRequest.Price = 1000;
					using (Socket socket = NolClient.GetSyncSocket())
					{
						FixmlMsg.DebugFormattedXml.Enabled = true;  // <- wyświetli nam dokładną treść komunikatów
						newRequest.Send(socket);
						execReport = new ExecutionReportMsg(socket);
						FixmlMsg.DebugFormattedXml.Enabled = tmp;
					}
					Thread.Sleep(3000);

					// --- modyfikacja tego zlecenia ---
					Console.WriteLine("\nPress any key... to MODIFY this order request    [Esc - exit]\n");
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					OrderReplaceRequestMsg replaceRequest = new OrderReplaceRequestMsg();
					replaceRequest.BrokerOrderId2 = execReport.BrokerOrderId2;
					replaceRequest.Account = accountNumber;
					replaceRequest.Side = OrderSide.Buy;
					replaceRequest.Instrument = newRequest.Instrument;
					replaceRequest.Quantity = 1;
					replaceRequest.Price = 1001;
					using (Socket socket = NolClient.GetSyncSocket())
					{
						FixmlMsg.DebugFormattedXml.Enabled = true;
						replaceRequest.Send(socket);
						execReport = new ExecutionReportMsg(socket);
						FixmlMsg.DebugFormattedXml.Enabled = tmp;
					}
					Thread.Sleep(3000);

					// --- anulowanie tego zlecenia ---
					Console.WriteLine("\nPress any key... to CANCEL this order request    [Esc - exit]\n");
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					OrderCancelRequestMsg cancelRequest = new OrderCancelRequestMsg();
					cancelRequest.BrokerOrderId2 = replaceRequest.BrokerOrderId2;
					cancelRequest.Account = accountNumber;
					cancelRequest.Side = newRequest.Side;
					cancelRequest.Instrument = newRequest.Instrument;
					cancelRequest.Quantity = newRequest.Quantity;
					using (Socket socket = NolClient.GetSyncSocket())
					{
						FixmlMsg.DebugFormattedXml.Enabled = true;
						cancelRequest.Send(socket);
						execReport = new ExecutionReportMsg(socket);
						FixmlMsg.DebugFormattedXml.Enabled = false;
					}
					Thread.Sleep(3000);

					Console.WriteLine("\nPress any key... to exit\n");
					Console.ReadKey(true);
					Console.WriteLine("\n\nThank you :)\n");
				}
				catch (Exception e)
				{
					MyUtil.PrintError(e);
				}
				FixmlMsg.DebugFormattedXml.Enabled = tmp;
			}  // tu następuje wylogowanie
		}


	}
}
