using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class BosAccountDataEx : IDemoModule
	{
		public char MenuKey { get { return '6'; } }
		public string Description { get { return "BossaAPI basics, account info using OnUpdate event"; } }

		public void Execute()
		{
			// połączenie z NOLem, zalogowanie użytkownika
			Bossa.ConnectNOL3();
			try
			{
				// podpięcie zdarzenia, po którym będziemy mogli odczytać nowy stan rachunku
				Bossa.OnUpdate += HandleBossaUpdate;

				// w tle odbieramy zdarzenia i wyświetlamy informacje o zmienionym stanie rachunku
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

		void HandleBossaUpdate(object obj, EventArgs e)
		{
			var account = obj as BosAccount;
			if (account != null)
			{
				Trace.WriteLine("");
				Trace.WriteLine(string.Format("Account: {0}", account.Number));
				Trace.WriteLine(string.Format("- porfolio value: {0}", account.PortfolioValue));
				Trace.WriteLine(string.Format("- deposit blocked: {0}", account.DepositBlocked));
				Trace.WriteLine(string.Format("- available funds: {0}", account.AvailableFunds));
				Trace.WriteLine(string.Format("- available cash: {0}", account.AvailableCash));
				// spis papierów wartościowych na danym rachunku
				if (account.Papers.Count > 0)
				{
					Trace.WriteLine("- papers: ");
					foreach (var paper in account.Papers)
						Trace.WriteLine(string.Format(" {0,5} x {1}", paper.Quantity, paper.Instrument));
				}
				// spis aktywnych zleceń na tym rachunku
				if (account.Orders.Count > 0)
				{
					Trace.WriteLine("- orders: ");
					foreach (var order in account.Orders)
						Trace.WriteLine(string.Format("  {0}: {1} {2} x {3} - {4}",
							order.Instrument, order.Side, order.Quantity, order.Price, order.StatusReport));
				}
				Trace.WriteLine("");
			}
		}
	}
}
