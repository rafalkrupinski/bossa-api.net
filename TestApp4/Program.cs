using System;
using pjank.BossaAPI;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.TestApp4
{
	/// <summary>
	/// 
	/// Poniższy przykład pokazuje, jak łączyć używanie klasy "Bossa" z bardziej niskopoziomowym dostępem do FIXML.
	/// 
	/// Jest to prosta rozbudowa przykładu z "TestApp3" o równoległy odczyt również bieżącego statusu sesji,
	/// który nie jest (póki co) dostępny bezpośrednio z "zewnętrznej" warstwy biblioteki (z klasy "Bossa"), 
	/// ale można go uzyskać wpinając się głębiej do "NolClient", otwierającej dostęp do całego protokołu FIXML.
	/// 
	/// </summary>
	class Program
	{
		/// <summary>
		/// Główna funkcja programu.
		/// </summary>
		/// <param name="args">
		/// lista przekazanych parametrów: podawane po spacji symbole kolejnych intrumentów
		/// </param>
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("Nie podano parametrów, włączam przykładową listę instrumentów...");
				args = new[] { "WIG20", "KGHM", "FW20H12" };
			}

			try
			{
				// podłączenie naszego handlera pod zdarzenie aktualizacji danych
				Bossa.OnUpdate += Bossa_OnUpdate;

				// aktywacja subskrypcji dla wybranych instrumentów
				// (praktycznie wystarczy dowolne odwołanie do danego symbolu na liście Instruments[...],
				// automatycznie uwzględnia również wszelkie instrumenty znajdujące się już na rachunku)
				foreach (var symbol in args)
					Bossa.Instruments[symbol].UpdatesEnabled = true;

				// własnoręczne przygotowanie obiektu IBosClient do komunikacji z NOL'em...
				var client = new NolClient();
				// i podpięcie się bezpośrednio do jednego z wielu jego wewnętrznych zdarzeń
				client.SessionStatusMsgEvent += NolClient_SessionStatusMsgEvent;
				client.TradingSessionStatusStart();

				// uruchomienie połączenia z NOL'em
				Bossa.Connect(client);

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

		/// <summary>
		/// Zdarzenie wywoływane przez NolClient po zmianie statusu konkretnego instrumentu (lub całego rynku?)
		/// </summary>
		/// <param name="msg">komunikat FIXML "TrdgSesStat"</param>
		private static void NolClient_SessionStatusMsgEvent(TradingSessionStatusMsg msg)
		{
			var symbol = msg.Instrument != null ? msg.Instrument.Symbol : null;
			Console.WriteLine("{0,-10} : {1}", symbol, msg.SessionPhase);
		}
	}
}
