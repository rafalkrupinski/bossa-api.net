using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using pjank.BossaAPI.DemoConsole.Modules;
using pjank.BossaAPI.DemoConsole.Utils;

namespace pjank.BossaAPI.DemoConsole
{
	class DemoConsole
	{
		/// <summary>
		/// Główna funkcja programu.
		/// </summary>
		public static void Main(string[] args)
		{
			using (new ConsoleLog("debug.log"))  // patrz dodatkowe opcje logowania w pliku .config
			{
				Trace.WriteLine("Hello!  [" + DateTime.Now + "]\n");
				try
				{
					LoadModules();
					IDemoModule module;
					while (SelectModule(out module))
					{
						if (module != null) ExecuteModule(module);
					}
				}
				finally
				{
					Trace.WriteLine("\nBye!  [" + DateTime.Now + "]\n");
					Console.ReadKey(true);
				}
			}
		}

		// lista załadowanych modułów demonstracyjnych
		private static IDemoModule[] modules;

		/// <summary>
		/// Instancjuje wszystkie moduły zdefiniowane w projekcie.
		/// </summary>
		private static void LoadModules()
		{
			modules = typeof(DemoConsole).Assembly.GetTypes()
				.Where(type => type.IsClass && typeof(IDemoModule).IsAssignableFrom(type))
				.Select(type => (IDemoModule)Activator.CreateInstance(type))
				.OrderBy(module => module.MenuKey)
				.ToArray();
		}

		/// <summary>
		/// Wyświetla menu dostępnych modułów i czeka na wybór użytkownika.
		/// </summary>
		/// <param name="module">zwraca wybrany moduł (null, jeśli wybór był nieprawidłowy)</param>
		/// <returns>zwraca False, jeśli naciśnięto Escape</returns>
		private static bool SelectModule(out IDemoModule module)
		{
			Console.WriteLine("\n\nSelect module:   (Esc - exit)");
			foreach (var m in modules)
				Console.WriteLine("{0} - {1}", m.MenuKey, m.Description);

			var keyInfo = Console.ReadKey(true);
			Trace.WriteLine("\nMenu selection: " + keyInfo.Key);

			var selectedKey = keyInfo.KeyChar.ToString().ToUpper()[0];
			module = modules.SingleOrDefault(m => m.MenuKey == selectedKey);

			return (keyInfo.Key != ConsoleKey.Escape);
		}

		/// <summary>
		/// Uruchamia wskazany moduł testowy.
		/// </summary>
		private static void ExecuteModule(IDemoModule module)
		{
			Debug.WriteLine("Module: " + module.GetType().Name);
			try
			{
				module.Execute();
			}
			catch (Exception e)
			{
				MyUtil.PrintError(e);
			}
		}
	}
}
