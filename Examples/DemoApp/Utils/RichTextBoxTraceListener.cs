using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace pjank.BossaAPI.DemoApp
{
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
