using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using pjank.BossaAPI;
using System.Diagnostics;
using pjank.BossaAPI.Fixml;

namespace pjank.BossaAPI.TestApp2
{
	public partial class Form1 : Form
	{
		TextBoxTraceListener myTraceListener;

		public Form1()
		{
			InitializeComponent();
			debugCheck1.Checked = FixmlMsg.DebugInternals.Enabled;
			debugCheck2.Checked = FixmlMsg.DebugOriginalXml.Enabled;
			debugCheck3.Checked = FixmlMsg.DebugFormattedXml.Enabled;
			debugCheck4.Checked = FixmlMsg.DebugParsedMessage.Enabled = true;
			myTraceListener = new RichTextBoxTraceListener(debugBox);
			Debug.Listeners.Add(myTraceListener);
			Bossa.Updated += new Action(Bossa_Updated);
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Debug.Listeners.Remove(myTraceListener);
		}


		// ----- obsługa okienka diagnostycznego ----- 

		private void debugScrollCheck_CheckedChanged(object sender, EventArgs e)
		{
			myTraceListener.AutoScroll = debugScrollCheck.Checked;
		}

		private void debugCheck1_CheckedChanged(object sender, EventArgs e)
		{
			FixmlMsg.DebugInternals.Enabled = debugCheck1.Checked;
		}
		private void debugCheck2_CheckedChanged(object sender, EventArgs e)
		{
			FixmlMsg.DebugOriginalXml.Enabled = debugCheck2.Checked;
		}
		private void debugCheck3_CheckedChanged(object sender, EventArgs e)
		{
			FixmlMsg.DebugFormattedXml.Enabled = debugCheck3.Checked;
		}
		private void debugCheck4_CheckedChanged(object sender, EventArgs e)
		{
			FixmlMsg.DebugParsedMessage.Enabled = debugCheck4.Checked;
		}

		private void debugBox_MouseEnter(object sender, EventArgs e)
		{
			debugOptions.Visible = true;
		}

		private void debugBox_MouseLeave(object sender, EventArgs e)
		{
			var pt = debugPanel.PointToClient(Cursor.Position);
			if (!debugPanel.ClientRectangle.Contains(pt))
				debugOptions.Visible = false;
		}


		// ----- obsługa przycisków -----

		private void ConnectBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (Bossa.IsConnected)
				{
					Debug.WriteLine("\nAlready connected!");
					return;
				}
				Bossa.ConnectNOL3();
			}
			catch (Exception ex)
			{
				ex.PrintError();
			}
		}

		private void DisconnectBtn_Click(object sender, EventArgs e)
		{
			try
			{
				if (!Bossa.IsConnected)
				{
					Debug.WriteLine("\nNot connected!");
					return;
				}
				Bossa.Disconnect();
			}
			catch (Exception ex)
			{
				ex.PrintError();
			}
		}


		// ----- listview z informacjami o rachunkach ----- 

		private void AddAccountGroup(BosAccount account)
		{
			var group = new ListViewGroup("Account: " + account.Number);
			accountsView.Groups.Add(group);
			foreach (var paper in account.Papers)
				AddAccountPaperItem(group, paper);
			AddAccountFundItem(group, "Cash:", account.Cash);
		}

		private void AddAccountPaperItem(ListViewGroup group, BosPaper paper)
		{
			var item = new ListViewItem(group);
			item.Text = paper.Instrument.ToString();
			item.SubItems.Add(paper.Quantity.ToString());
			accountsView.Items.Add(item);
		}

		private void AddAccountFundItem(ListViewGroup group, string text, decimal value)
		{
			var item = new ListViewItem(group);
			item.ForeColor = Color.Brown;
			item.Text = text;
			item.SubItems.Add(value.ToString());
			accountsView.Items.Add(item);
		}

		private void AddAccountOrderItem(ListViewGroup group, BosOrder order)
		{
			var item = new ListViewItem(group);
			item.ForeColor = Color.Blue;
			item.Text = order.Side.ToString() + " " + order.Instrument.ToString();
			item.SubItems.Add(order.Quantity.ToString());
			item.SubItems.Add(string.Format("x {0:0.00} {1:(0.00)}", order.Price, order.ActivationPrice));
			accountsView.Items.Add(item);
		}

		void Bossa_Updated()
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(Bossa_Updated));
			else
			{
				accountsView.BeginUpdate();
				try
				{
					accountsView.Items.Clear();
					foreach (var account in Bossa.Accounts)
						AddAccountGroup(account);
				}
				finally
				{
					accountsView.EndUpdate();
				}
			}
		}
	}
}
