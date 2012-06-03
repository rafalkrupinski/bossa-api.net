using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class NolCustomOrders : IDemoModule
	{
		public char MenuKey { get { return '3'; } }
		public string Description { get { return "NolClient usage, sending orders directly with Fixml classes"; } }

		/// <summary>
		/// Przykład wysyłki zleceń, korzystając bezpośrednio z klas NewOrderSingleMsg i spółki...
		/// Test ten wrzuca na giełdę zlecenie kupna 1 x FW20 po 1000zł (*raczej* nie ma szans się zrealizować :)),
		/// następnie je modyfikuje ustawiając limit ceny oczko wyżej... aż ostatecznie całe zlecenie anuluje. 
		/// </summary>
		public void Execute()
		{
			var accountNumber = "00-22-...";  // <- wpisz tu swój numer, żeby program nie musiał o niego pytać
			if (accountNumber.EndsWith("..."))
			{
				Console.Write("Podaj numer rachunku (końcówkę z " + accountNumber + "): ");
				var str = Console.ReadLine();
				accountNumber = accountNumber.Replace("...", str);
				Trace.WriteLine("Wybrany rachunek: " + accountNumber);
			}

			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (var nol = new NolClient())
			{
				Thread.Sleep(2000);
				var tmp = FixmlMsg.DebugFormattedXml.Enabled;
				try
				{
					ExecutionReportMsg execReport;

					// --- wysyłka nowego zlecenia --- 
					Console.WriteLine("\nPress any key... to send NEW order request    [Esc - exit]\n");
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					var newRequest = new NewOrderSingleMsg();
					newRequest.Account = accountNumber;
					newRequest.Side = OrderSide.Buy;
					newRequest.Instrument = FixmlInstrument.FindBySym("FW20H12");
					newRequest.Quantity = 1;
					newRequest.Price = 1000;
					using (var socket = NolClient.GetSyncSocket())
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

					var replaceRequest = new OrderReplaceRequestMsg();
					replaceRequest.BrokerOrderId2 = execReport.BrokerOrderId2;
					replaceRequest.Account = accountNumber;
					replaceRequest.Side = OrderSide.Buy;
					replaceRequest.Instrument = newRequest.Instrument;
					replaceRequest.Quantity = 1;
					replaceRequest.Price = 1001;
					using (var socket = NolClient.GetSyncSocket())
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

					var cancelRequest = new OrderCancelRequestMsg();
					cancelRequest.BrokerOrderId2 = replaceRequest.BrokerOrderId2;
					cancelRequest.Account = accountNumber;
					cancelRequest.Side = newRequest.Side;
					cancelRequest.Instrument = newRequest.Instrument;
					cancelRequest.Quantity = newRequest.Quantity;
					using (var socket = NolClient.GetSyncSocket())
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
