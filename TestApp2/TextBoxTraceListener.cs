using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace pjank.BossaAPI.TestApp2
{
	public abstract class GuiTraceListener : TraceListener
	{
		protected Control control;

		public GuiTraceListener(Control outputControl)
		{
			control = outputControl;
		}

		public override void Write(string message)
		{
			control.BeginInvoke((MethodInvoker)delegate { DoWrite(message); });
		}
		public override void WriteLine(string message)
		{
			Write(message + "\n");
		}

		protected abstract void DoWrite(string message);

		protected Dictionary<string, Color> colorMap = new Dictionary<string, Color>
                {
                    { "##ERROR##", Color.Red },
                    { "#WARNING#", Color.DarkRed },
                    { "fixml:", Color.Gray },
                    { "fixml-send:", Color.Teal },
                    { "fixml-recv:", Color.Blue },
                };

		protected Color GetTextColor(string message)
		{
			foreach (var item in colorMap)
				if (message.TrimStart('\n').StartsWith(item.Key))
					return item.Value;
			return control.ForeColor;
		}
	}


	public class TextBoxTraceListener : GuiTraceListener
	{
		public bool AutoScroll { get; set; }

		public TextBoxTraceListener(TextBoxBase output)
			: base(output)
		{
			AutoScroll = true;
		}

		private TextBoxBase textBox { get { return (TextBoxBase)base.control; } }

		protected override void DoWrite(string message)
		{
			textBox.AppendText(message);
			if (AutoScroll)
				textBox.ScrollToCaret();
		}
	}


	public class RichTextBoxTraceListener : TextBoxTraceListener
	{
		public RichTextBoxTraceListener(RichTextBox output) : base(output) { }

		public Dictionary<string, Color> ColorMap { get { return colorMap; } }

		private RichTextBox textBox { get { return (RichTextBox)base.control; } }

		protected override void DoWrite(string message)
		{
			textBox.Select(textBox.TextLength, 0);
			textBox.SelectionColor = GetTextColor(message);
			base.DoWrite(message);
			textBox.SelectionColor = textBox.ForeColor;
		}
	}
}
