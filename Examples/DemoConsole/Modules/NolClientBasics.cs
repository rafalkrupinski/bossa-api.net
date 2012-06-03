using System;
using System.Threading;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.DemoConsole.Modules
{
	class NolClientBasics : IDemoModule
	{
		public char MenuKey { get { return '1'; } }
		public string Description { get { return "NolClient basics, login/logout, sample message handler"; } }

		/// <summary>
		/// Proste zalogowanie i wylogowanie użytkownika z użyciem klasy "NolClient".
		/// Przykład podpięcia zdarzenia pod wybrany rodzaj komunikatu asynchronicznego.
		/// </summary>
		public void Execute()
		{
			// nawiązanie połączenia z NOL3 i zalogowanie użytkownika
			using (var nol = new NolClient())
			{
				// podpinamy się pod konkretny rodzaj komunikatów, tutaj - wyciąg z rachunku
				nol.StatementMsgEvent += nol_StatementMsgHandler;

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
		/// Przykład obsługi odebranego w klasie NolClient komunikatu asynchronicznego.
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
	}
}
