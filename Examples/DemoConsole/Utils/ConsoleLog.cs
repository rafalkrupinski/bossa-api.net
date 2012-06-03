using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace pjank.BossaAPI.DemoConsole.Utils
{
	/// <summary>
	/// Klasa przechwytuje wszystko wysyłane do Trace/Debug,
	/// zapisując to jednocześnie w oknie konsoli i do pliku (zachowuje kilka ostatnich plików).
	/// </summary>
	public class ConsoleLog : IDisposable
	{
		private readonly string filename;

		public ConsoleLog(string debugFileName)
		{
			filename = debugFileName;
			MoveOldFiles();
			InitConsole();
			InitListeners();
		}

		public void Dispose()
		{
			Debug.Flush();
			Debug.Close();
		}

		private void InitConsole()
		{
			Console.BufferWidth = 1000;
			// (wskazana mała czcionka dla okna konsoli... ewentualnie wyłączyć poniższe)
			Console.WindowWidth = Math.Min(140, Console.LargestWindowWidth);
			Console.WindowHeight = Math.Min(60, Console.LargestWindowHeight);
		}

		private void InitListeners()
		{
			Debug.Listeners.Add(new TextWriterTraceListener(filename));
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
		}

		private void MoveOldFiles()
		{
			if (File.Exists(filename + 2)) File.Delete(filename + 2);
			if (File.Exists(filename + 1)) File.Move(filename + 1, filename + 2);
			if (File.Exists(filename)) File.Move(filename, filename + 1);
		}

	}
}
