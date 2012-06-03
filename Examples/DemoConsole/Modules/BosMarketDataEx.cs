using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class BosMarketDataEx : IDemoModule
	{
		public char MenuKey { get { return '8'; } }
		public string Description { get { return "BossaAPI+NolClient, combined with low-level Fixml events"; } }

		public void Execute()
		{
			try
			{
				// podpięcie zdarzenia zmiany notowań
				Bossa.OnUpdate += HandleBossaUpdate;

				// włączenie odbioru notowań dla wybranych instrumentów
				// (wystarczy samo odwołanie się do konkretnego obiektu 'Bossa.Instruments[...]')
				Bossa.Instruments["KGHM"].UpdatesEnabled = true;
				Bossa.Instruments["WIG20"].UpdatesEnabled = true;
				Bossa.Instruments["FW20M12"].UpdatesEnabled = true;

				// własnoręczne przygotowanie obiektu do komunikacji z NOL'em...
				var client = new NolClient();
				// i podpięcie się bezpośrednio do jednego z jego wewnętrznych zdarzeń
				client.SessionStatusMsgEvent += HandleSessionStatusMsgEvent;

				// uruchomienie połączenia (to zamiast standardowego "ConnectNOL3")
				Bossa.Connect(client);

				// w tle odbieramy zdarzenia i wyświetlamy bieżące notowania
				Console.WriteLine("Press any key... to exit");
				Console.ReadKey(true);
			}
			finally
			{
				Bossa.OnUpdate -= HandleBossaUpdate;
				Bossa.Disconnect();
				Bossa.Clear();
			}
		}

		// wywoływane przy aktualizacji danych rynkowych lub stanu rachunku.
		void HandleBossaUpdate(object obj, EventArgs e)
		{
			var ins = obj as BosInstrument;
			// wyświetlenie bieżących notowań: nalepsze oferty, parametry ostatniej transakcji
			if (ins != null)
			{
				Trace.WriteLine(string.Format("{0,-10} [ {1,-8} {2,8} ] {3}",
					ins.Symbol, ins.BuyOffers.BestPrice, ins.SellOffers.BestPrice, ins.Trades.Last));
			}
		}

		// wywoływane przez NolClient po otrzymaniu komunikatu 'TrdgSesStat'
		void HandleSessionStatusMsgEvent(TradingSessionStatusMsg msg)
		{
			var symbol = msg.Instrument != null ? msg.Instrument.Symbol : null;
			Trace.WriteLine(string.Format("{0,-10} {1}", symbol, msg.SessionPhase));
		}
	}
}
