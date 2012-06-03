using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Xml;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class NolCustomFixml : IDemoModule
	{
		public char MenuKey { get { return '4'; } }
		public string Description { get { return "NolClient usage, custom login and message handling"; } }

		/// <summary>
		/// Bardziej zaawansowany przykład użycia "NolClient": zalogowanie dopiero na żądanie, 
		/// bez uruchamiania wątku odbierającego komunikaty asynchroniczne (można go obsłużyć samemu).
		/// Samodzielne przygotowanie, wysyłka i odbiór przykładowego message'a.
		/// </summary>
		public void Execute()
		{
			using (var nol = new NolClient(false, false))
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
				var tmp = FixmlMsg.DebugOriginalXml.Enabled;
				try
				{
					FixmlMsg.DebugOriginalXml.Enabled = true;

					// otwarcie nowego połączenia (kanał synchroniczny za każdym razem nowy!)
					using (var syncSocket = NolClient.GetSyncSocket())
					{
						// przygotowanie komunikatu
						var request = new UserRequestMsg()
						{
							Username = "BOS",
							Type = UserRequestType.GetStatus,
						};
						// wysyłka komunikatu
						request.Send(syncSocket);

						// odbiór odpowiedzi
						Console.WriteLine("\nPress any key... to read the response\n");
						Console.ReadKey(true);
						var response = new FixmlMsg(syncSocket);
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
					using (var syncSocket = NolClient.GetSyncSocket())
					{
						// tak można spreparować dowolny komunikat, również taki jeszcze nieistniejący ;->
						var request = new CustomMsg("MyCustomRequest");
						var xmlElement = request.AddElement("Test");
						xmlElement.SetAttribute("attr1", "1");
						xmlElement.SetAttribute("attr2", "2");
						// wysyłka tak samodzielnie spreparowanego komunikatu
						request.Send(syncSocket);
						// odbiór odpowiedzi - tutaj powinniśmy otrzymać błąd... "BizMessageRejectException"
						// niestety aktualna wersja NOL3 zwraca nieprawidłowy XML, którego nie da się parsować
						Console.WriteLine("\nPress any key... to read the response\n");
						Console.ReadKey(true);
						var response = new FixmlMsg(syncSocket);
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
	}
}
