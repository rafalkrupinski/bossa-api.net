using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace pjank.BossaAPI.TestApp2
{
	/// <summary>
	/// Klasa bazowa dla obiektów TraceListenera wyświetlających komunikaty na kontrolce GUI.
	/// Sama zajmuje się synchronizacją wątków
	///  (tych, z których przychodzą komunikaty, z wątkiem danej kontrolki docelowej) 
	/// oraz wewnętrznym kolejkowaniem tych komunikatów
	///  (żeby nie obciążać niepotrzebnie interfejsu aplikacji, kiedy "dużo się dzieje").
	/// </summary>
	public abstract class GuiTraceListener : TraceListener
	{
		protected Control control;
		private List<string> queue;
		private bool busy;

		// konstruktor, zapamiętuje docelową kontrolkę
		public GuiTraceListener(Control outputControl)
		{
			control = outputControl;
			queue = new List<string>();
			busy = false;
		}

		// metoda TraceListener'a - dopisuje wraz z dodatkowym znakiem końca wiersza
		public override void WriteLine(string message)
		{
			Write(message + "\n");
		}

		// metoda TraceListener'a - dopisuje dokładnie podany string
		public override void Write(string message)
		{
			lock (this)
			{
				queue.Add(message);
				if (busy) return;
				else busy = true;				
			}
			control.BeginInvoke(new MethodInvoker(DoWrite));
		}

		// liczba komunikatów w kolejce, po przekroczeniu której uruchamiamy "drugi bieg" ;)
		public const int BacklogLimit = 25;

		// wywoływane przez "Invoke" - w wątku docelowej kontrolki
		private void DoWrite()
		{
			string[] messages;
			lock (this)
			{
				messages = queue.ToArray();
				queue.Clear();
				busy = false;
			}
			// wydruk zebranych w kolejce komunikatów: każdy pojedynczo...
			if (messages.Length <= BacklogLimit)
				foreach (var message in messages)
					DoWrite(message, false);
			else  // albo, jeśli ich dużo, wszystkie razem - tak szybciej nadgonimy
				DoWrite(string.Concat(messages), true);
		}

		// w klasach pochodnych tutaj wykonujemy rzeczywiste dopisanie wierszy do kontrolki
		protected abstract void DoWrite(string message, bool backlog);

		// domyślna konfiguracja kolorków (używana, jeśli dana kontrolka wspiera kolorowanie)
		protected Dictionary<string, Color> colorMap = new Dictionary<string, Color>
                {
                    { "##ERROR##", Color.Red },
                    { "#WARNING#", Color.DarkRed },
                    { "fixml:", Color.Gray },
                    { "fixml-send:", Color.Teal },
                    { "fixml-recv:", Color.Blue },
                };

		// ustalenie koloru komunikatu (na podstawie jego początkowej treści)
		protected Color GetTextColor(string message)
		{
			foreach (var item in colorMap)
				if (message.TrimStart('\n').StartsWith(item.Key))
					return item.Value;
			return control.ForeColor;
		}
	}


	/// <summary>
	/// Wersja GuiTraceListenera przystosowana do zwykłego TextBox'a (bez kolorowania).
	/// </summary>
	public class TextBoxTraceListener : GuiTraceListener
	{
		public bool AutoScroll { get; set; }

		public TextBoxTraceListener(TextBoxBase output) : base(output)
		{
			AutoScroll = true;
		}

		private TextBoxBase textBox { get { return (TextBoxBase)base.control; } }

		protected override void DoWrite(string message, bool backlog)
		{
			textBox.AppendText(message);
			if (AutoScroll)
				textBox.ScrollToCaret();
		}
	}


	/// <summary>
	/// Wersja GuiTraceListener'a przystosowana do RichTextBox'a (obsługuje kolorowanie wierszy).
	/// </summary>
	public class RichTextBoxTraceListener : TextBoxTraceListener
	{
		public RichTextBoxTraceListener(RichTextBox output) : base(output) { }

		public Dictionary<string, Color> ColorMap { get { return colorMap; } }

		private RichTextBox textBox { get { return (RichTextBox)base.control; } }

		protected override void DoWrite(string message, bool backlog)
		{
			if (backlog)  // jeśli to zaległe wpisy - dopisujemy je szybko, bez kolorowania
				base.DoWrite(message, backlog);
			else
			{
				textBox.Select(textBox.TextLength, 0);
				textBox.SelectionColor = GetTextColor(message);
				base.DoWrite(message, backlog);
				textBox.SelectionColor = textBox.ForeColor;
			}
		}
	}
}
