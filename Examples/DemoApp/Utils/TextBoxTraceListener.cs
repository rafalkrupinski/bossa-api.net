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
}
