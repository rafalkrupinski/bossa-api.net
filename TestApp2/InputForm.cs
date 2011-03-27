using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pjank.BossaAPI.TestApp2
{
	public partial class InputForm : Form
	{
		public InputForm()
		{
			InitializeComponent();
		}

		public static string GetString(string title)
		{
			var form = new InputForm { Text = title };
			if (form.ShowDialog() != DialogResult.OK)
				return null;
			return form.TextBox.Text;
		}

		private void InputForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.Cancel;
		}
	}
}
