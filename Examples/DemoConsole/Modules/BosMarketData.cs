using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class BosMarketData : IDemoModule
	{
		public char MenuKey { get { return '7'; } }
		public string Description { get { return "BossaAPI basics, market data subscription"; } }

		public void Execute()
		{
			// połączenie z NOLem, zalogowanie użytkownika
			Bossa.ConnectNOL3();
			try
			{
				// podpięcie zdarzenia zmiany notowań
				Bossa.OnUpdate += HandleBossaUpdate;

				// włączenie odbioru notowań dla wybranych instrumentów
				// (wystarczy samo odwołanie się do konkretnego obiektu 'Bossa.Instruments[...]')
				Bossa.Instruments["KGHM"].UpdatesEnabled = true;
				Bossa.Instruments["WIG20"].UpdatesEnabled = true;
				Bossa.Instruments["FW20M12"].UpdatesEnabled = true;

				// w tle odbieramy zdarzenia i wyświetlamy notowania... na początek ze szczegółami...
				Console.WriteLine("Press any key... to disable OHLC details   (Esc - exit)");
				if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;

				showStats = false;  // wystarczy już tych "statystyk" sesji, bez tego będzie czytelniej

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

		private bool showStats = true;

		void HandleBossaUpdate(object obj, EventArgs e)
		{
			var ins = obj as BosInstrument;
			// wyświetlenie bieżących notowań: nalepsze oferty, parametry ostatniej transakcji
			if (ins != null)
			{
				Trace.WriteLine(string.Format("{0,-10} [ {1,-8} {2,8} ] {3}",
					ins.Symbol, ins.BuyOffers.BestPrice, ins.SellOffers.BestPrice, ins.Trades.Last));
			}

			// ewentualne dalsze szczegóły...
			if (ins != null && showStats)
			{
				var s = ins.Session;
				Trace.WriteLine(string.Format(" open  = {0} ({1:0.0}mln)", s.OpeningPrice, s.OpeningTurnover / 1000000));
				Trace.WriteLine(string.Format(" close = {0} ({1:0.0}mln)", s.ClosingPrice, s.ClosingTurnover / 1000000));
				Trace.WriteLine(string.Format(" high  = {0}", s.HighestPrice));
				Trace.WriteLine(string.Format(" low   = {0}", s.LowestPrice));
				Trace.WriteLine(string.Format(" ref   = {0}", s.ReferencePrice));
				Trace.WriteLine(string.Format(" vol   = {0} ({1:0.0}mln)", s.TotalVolume, s.TotalTurnover / 1000000));
				Trace.WriteLine("");
			}
		}
	}
}
