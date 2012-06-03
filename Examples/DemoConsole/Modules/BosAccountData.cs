using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class BosAccountData : IDemoModule
	{
		public char MenuKey { get { return '5'; } }
		public string Description { get { return "BossaAPI basics, account info"; } }

		public void Execute()
		{
			// połączenie z NOLem, zalogowanie użytkownika
			Bossa.ConnectNOL3();
			try
			{
				// tu powinniśmy chwilę zaczekać, aż NOL przyśle nam "wyciąg"
				Console.WriteLine("Press any key... to read account info");
				Console.ReadKey(true);
				Console.WriteLine();

				// wyświetlenie informacji o dostępnych rachunkach
				foreach (var account in Bossa.Accounts)
				{
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

				Console.WriteLine("Press any key... to exit");
				Console.ReadKey(true);
			}
			finally
			{
				Bossa.Disconnect();
				Bossa.Clear();
			}
		}
	}
}
