using System;
using pjank.BossaAPI;

namespace pjank.BossaAPI.TestApp3
{
	class Program
	{
		/// <summary>
		/// Główna funkcja programu.
		/// </summary>
		/// <param name="args">
		/// lista przekazanych parametrów: podawane po spacji symbole kolejnych intrumentów
		/// </param>
		public static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Nie podano parametrów, włączam przykładową listę instrumentów...");
				args = new[] { "WIG20", "KHGM", "FW20H12" };
			}

			try
			{
				// podłączenie naszego handlera pod zdarzenie aktualizacji danych
				Bossa.OnUpdate += new EventHandler(Bossa_OnUpdate);

				// aktywacja subskrypcji dla wybranych instrumentów
				// (praktycznie wystarczy dowolne odwołanie do danego symbolu na liście Instruments[...],
				// automatycznie uwzględnia również wszelkie instrumenty znajdujące się już na rachunku)
				foreach (var symbol in args)
					Bossa.Instruments[symbol].UpdatesEnabled = true;

				// uruchomienie połączenia z NOL'em
				Bossa.ConnectNOL3();

				// czekamy na naciśnięcie dowolnego klawisza (w tle będzie odpalał Bossa_OnUpdate)
				Console.ReadKey(true);

				// zakończenie połączenia z NOL'em
				Bossa.Disconnect();

			}
			catch (Exception e)
			{
				// przechwycenie ew. błędów (np. komunikacji z NOL'em) - wyświetlenie komunikatu
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// Zdarzenie wywoływane przy aktualizacji danych rynkowych lub stanu rachunku.
		/// </summary>
		/// <param name="obj">zmieniony obiekt: BosInstrument lub BosAccount</param>
		/// <param name="e">dodatkowe parametry, aktualnie nieużywane</param>
		private static void Bossa_OnUpdate(object obj, EventArgs e)
		{
			var ins = obj as BosInstrument;
			if (ins != null)  // wyświetlenie w kolejnym wierszu bieżących notowań
				Console.WriteLine("{0,-10} [ {1,-8} {2,8} ] {3}", ins.Symbol,
				  ins.BuyOffers.BestPrice, ins.SellOffers.BestPrice, ins.Trades.Last);
		}
	}
}
