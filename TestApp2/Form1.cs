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
			Bossa.OnUpdate += new EventHandler(Bossa_OnUpdate);
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


		// ----- ustawianie listview z informacjami o rachunkach ----- 

		private void UpdateAccountInfo(BosAccount account)
		{
			accountsView.BeginUpdate();
			try
			{
				var group = GetAccountGroup(account);
				foreach (var paper in account.Papers)
					AddAccountPaperItem(group, paper);
				foreach (var order in account.Orders)
					if (order.IsActive) AddAccountOrderItem(group, order);
				AddAccountFundItem(group, "Total", account.PortfolioValue, null, null);
				AddAccountFundItem(group, "Deposit", account.DepositBlocked, "deficit", account.DepositDeficit);
				AddAccountFundItem(group, "Available", account.AvailableFunds, "cash", account.AvailableFunds);
			}
			finally
			{
				accountsView.EndUpdate();
			}
		}

		private ListViewGroup GetAccountGroup(BosAccount account)
		{
			var group = accountsView.Groups[account.Number];
			if (group == null)
			{
				group = new ListViewGroup(account.Number, "Account: " + account.Number);
				accountsView.Groups.Add(group);
			}
			else
			{
				var oldItems = group.Items.OfType<ListViewItem>().ToArray();
				foreach (var item in oldItems)
					accountsView.Items.Remove(item);
			}
			return group;
		}

		private void AddAccountPaperItem(ListViewGroup group, BosPaper paper)
		{
			var item = new ListViewItem(group);
			item.Text = paper.Instrument.ToString();
			item.SubItems.Add(paper.Quantity.ToString());
			accountsView.Items.Add(item);
		}

		private void AddAccountFundItem(ListViewGroup group, string text, decimal? value, string text2, decimal? value2)
		{
			if (value == null) return;
			var item = new ListViewItem(group);
			item.ForeColor = Color.Brown;
			item.Text = string.Format("{0}:", text);
			item.SubItems.Add(value.ToString());
			if (value2 != null && value2 != value) 
				item.SubItems.Add(string.Format("{0}: {1}", text2, value2));
			accountsView.Items.Add(item);
		}

		private void AddAccountOrderItem(ListViewGroup group, BosOrder order)
		{
			var item = new ListViewItem(group);
			item.ForeColor = Color.Blue;
			item.Text = order.Instrument.ToString();
			item.SubItems.Add(string.Format("{0}{1}  x", 
				(order.Side == BosOrderSide.Buy) ? "+" : "- ", order.Quantity.ToString()));
			item.SubItems.Add(string.Format("{0:0.00} {1:(0.00)}", order.Price, order.ActivationPrice));
			item.SubItems.Add(order.StatusReport.Status.ToString());
			accountsView.Items.Add(item);
		}



		// ----- ustawianie listview z notowaniami instrumentów ----- 

		private void UpdateInstrumentInfo(BosInstrument instrument)
		{
			var item = GetInstrumentItem(instrument);
			item.Name = instrument.ISIN ?? instrument.Symbol;
			item.Text = instrument.Symbol ?? instrument.ISIN;
			var bid = instrument.BuyOffers.Best;
			item.SubItems.Add((bid != null) ? bid.Volume.ToString() : "");
			item.SubItems.Add((bid != null) ? bid.Price.ToString() : "");
			var ask = instrument.SellOffers.Best;
			item.SubItems.Add((ask != null) ? ask.Price.ToString() : "");
			item.SubItems.Add((ask != null) ? ask.Volume.ToString() : "");
			var trd = instrument.Trades.Last;
			item.SubItems.Add((trd != null) ? trd.Quantity.ToString() : "");
			item.SubItems.Add((trd != null) ? trd.Price.ToString() : "");
			item.SubItems.Add((trd != null) ? trd.Time.TimeOfDay.ToString() : "");
		}

		private ListViewItem GetInstrumentItem(BosInstrument instrument)
		{
			var item = instrumentsView.Items[instrument.ISIN];
			if (item == null)
				item = instrumentsView.Items[instrument.Symbol];
			if (item == null)
			{
				item = new ListViewItem();
				item.Tag = instrument;
				instrumentsView.Items.Add(item);
			}
			return item;
		}


		// ----- odczyt modyfikacji jednego z obiektów w klasie Bossa ----- 

		void Bossa_OnUpdate(object obj, EventArgs e)
		{
			if (obj is BosAccount) UpdateAccountInfo((BosAccount)obj);
			if (obj is BosInstrument) UpdateInstrumentInfo((BosInstrument)obj);
		}
	}
}
