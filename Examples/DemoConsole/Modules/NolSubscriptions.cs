using System;
using System.Diagnostics;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class NolSubscriptions : IDemoModule
	{
		public char MenuKey { get { return '2'; } }
		public string Description { get { return "NolClient usage, MarketData and SessionStatus subscriptions"; } }

		/// <summary>
		/// Wykorzystanie klasy NolClient do subskrypcji bieżących notowań wybranych papierów wartościowych.
		/// </summary>
		public void Execute()
		{
			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (var nol = new NolClient())
			{
				try
				{
					if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

					Trace.WriteLine("\n--- market data subscription start ---");

					// definiujemy filtr subskrybowanych danych 
					nol.MarketDataSubscriptionAdd(MDEntryTypes.BasicBook);
					nol.MarketDataSubscriptionAdd(MDEntryTypes.BasicTrade);
					FixmlInstrument fw20 = FixmlInstrument.FindBySym("FW20H13");
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
					nol.MarketDataSubscriptionAdd("WIG20", "FW20M12", "FW20H13");
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
	}
}
