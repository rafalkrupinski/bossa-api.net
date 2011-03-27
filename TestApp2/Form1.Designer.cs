namespace pjank.BossaAPI.TestApp2
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.accTextColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.accValueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.accPriceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.accStatusColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insSymbolColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insBidVolColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insBidPriceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insAskPriceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insAskVolColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insTradeTimeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insTradeVolColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.insTradePriceColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.DisconnectBtn = new System.Windows.Forms.Button();
			this.ConnectBtn = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.debugCheck1 = new System.Windows.Forms.CheckBox();
			this.debugCheck2 = new System.Windows.Forms.CheckBox();
			this.debugCheck3 = new System.Windows.Forms.CheckBox();
			this.debugCheck4 = new System.Windows.Forms.CheckBox();
			this.debugPanel = new System.Windows.Forms.Panel();
			this.debugOptions = new System.Windows.Forms.FlowLayoutPanel();
			this.debugOptionsLabel = new System.Windows.Forms.Label();
			this.debugScrollCheck = new System.Windows.Forms.CheckBox();
			this.debugBox = new System.Windows.Forms.RichTextBox();
			this.accountsView = new System.Windows.Forms.ListView();
			this.instrumentsView = new System.Windows.Forms.ListView();
			this.debugPanel.SuspendLayout();
			this.debugOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// accTextColumn
			// 
			this.accTextColumn.Text = "Text";
			// 
			// accValueColumn
			// 
			this.accValueColumn.Text = "Value";
			this.accValueColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// accPriceColumn
			// 
			this.accPriceColumn.Text = "Price";
			this.accPriceColumn.Width = 80;
			// 
			// accStatusColumn
			// 
			this.accStatusColumn.Text = "Status";
			// 
			// insSymbolColumn
			// 
			this.insSymbolColumn.Text = "Symbol";
			// 
			// insBidVolColumn
			// 
			this.insBidVolColumn.Text = "";
			this.insBidVolColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.insBidVolColumn.Width = 40;
			// 
			// insBidPriceColumn
			// 
			this.insBidPriceColumn.Text = "Bid";
			this.insBidPriceColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.insBidPriceColumn.Width = 55;
			// 
			// insAskPriceColumn
			// 
			this.insAskPriceColumn.Text = "Ask";
			this.insAskPriceColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.insAskPriceColumn.Width = 55;
			// 
			// insAskVolColumn
			// 
			this.insAskVolColumn.Text = "";
			this.insAskVolColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.insAskVolColumn.Width = 40;
			// 
			// insTradeTimeColumn
			// 
			this.insTradeTimeColumn.Text = "";
			this.insTradeTimeColumn.Width = 54;
			// 
			// insTradeVolColumn
			// 
			this.insTradeVolColumn.Text = "";
			this.insTradeVolColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.insTradeVolColumn.Width = 45;
			// 
			// insTradePriceColumn
			// 
			this.insTradePriceColumn.Text = "Trade";
			this.insTradePriceColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// DisconnectBtn
			// 
			this.DisconnectBtn.Location = new System.Drawing.Point(93, 12);
			this.DisconnectBtn.Name = "DisconnectBtn";
			this.DisconnectBtn.Size = new System.Drawing.Size(75, 23);
			this.DisconnectBtn.TabIndex = 5;
			this.DisconnectBtn.Text = "Disconnect";
			this.DisconnectBtn.UseVisualStyleBackColor = true;
			this.DisconnectBtn.Click += new System.EventHandler(this.DisconnectBtn_Click);
			// 
			// ConnectBtn
			// 
			this.ConnectBtn.Location = new System.Drawing.Point(12, 12);
			this.ConnectBtn.Name = "ConnectBtn";
			this.ConnectBtn.Size = new System.Drawing.Size(75, 23);
			this.ConnectBtn.TabIndex = 4;
			this.ConnectBtn.Text = "Connect";
			this.ConnectBtn.UseVisualStyleBackColor = true;
			this.ConnectBtn.Click += new System.EventHandler(this.ConnectBtn_Click);
			// 
			// debugCheck1
			// 
			this.debugCheck1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.debugCheck1.AutoSize = true;
			this.debugCheck1.Location = new System.Drawing.Point(82, 4);
			this.debugCheck1.Name = "debugCheck1";
			this.debugCheck1.Size = new System.Drawing.Size(15, 14);
			this.debugCheck1.TabIndex = 13;
			this.toolTip1.SetToolTip(this.debugCheck1, "FixmlMsg Internals");
			this.debugCheck1.UseVisualStyleBackColor = true;
			this.debugCheck1.CheckedChanged += new System.EventHandler(this.debugCheck1_CheckedChanged);
			// 
			// debugCheck2
			// 
			this.debugCheck2.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.debugCheck2.AutoSize = true;
			this.debugCheck2.Location = new System.Drawing.Point(103, 4);
			this.debugCheck2.Name = "debugCheck2";
			this.debugCheck2.Size = new System.Drawing.Size(15, 14);
			this.debugCheck2.TabIndex = 14;
			this.toolTip1.SetToolTip(this.debugCheck2, "Original XML");
			this.debugCheck2.UseVisualStyleBackColor = true;
			this.debugCheck2.CheckedChanged += new System.EventHandler(this.debugCheck2_CheckedChanged);
			// 
			// debugCheck3
			// 
			this.debugCheck3.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.debugCheck3.AutoSize = true;
			this.debugCheck3.Location = new System.Drawing.Point(124, 4);
			this.debugCheck3.Name = "debugCheck3";
			this.debugCheck3.Size = new System.Drawing.Size(15, 14);
			this.debugCheck3.TabIndex = 15;
			this.toolTip1.SetToolTip(this.debugCheck3, "Formatted XML");
			this.debugCheck3.UseVisualStyleBackColor = true;
			this.debugCheck3.CheckedChanged += new System.EventHandler(this.debugCheck3_CheckedChanged);
			// 
			// debugCheck4
			// 
			this.debugCheck4.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.debugCheck4.AutoSize = true;
			this.debugCheck4.Location = new System.Drawing.Point(145, 4);
			this.debugCheck4.Name = "debugCheck4";
			this.debugCheck4.Size = new System.Drawing.Size(15, 14);
			this.debugCheck4.TabIndex = 16;
			this.toolTip1.SetToolTip(this.debugCheck4, "Parsed Message");
			this.debugCheck4.UseVisualStyleBackColor = true;
			this.debugCheck4.CheckedChanged += new System.EventHandler(this.debugCheck4_CheckedChanged);
			// 
			// debugPanel
			// 
			this.debugPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.debugPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.debugPanel.Controls.Add(this.debugOptions);
			this.debugPanel.Controls.Add(this.debugBox);
			this.debugPanel.Location = new System.Drawing.Point(12, 293);
			this.debugPanel.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
			this.debugPanel.Name = "debugPanel";
			this.debugPanel.Size = new System.Drawing.Size(685, 157);
			this.debugPanel.TabIndex = 9;
			// 
			// debugOptions
			// 
			this.debugOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.debugOptions.AutoSize = true;
			this.debugOptions.BackColor = System.Drawing.SystemColors.Info;
			this.debugOptions.Controls.Add(this.debugOptionsLabel);
			this.debugOptions.Controls.Add(this.debugCheck1);
			this.debugOptions.Controls.Add(this.debugCheck2);
			this.debugOptions.Controls.Add(this.debugCheck3);
			this.debugOptions.Controls.Add(this.debugCheck4);
			this.debugOptions.Controls.Add(this.debugScrollCheck);
			this.debugOptions.ForeColor = System.Drawing.Color.Black;
			this.debugOptions.Location = new System.Drawing.Point(406, 1);
			this.debugOptions.Name = "debugOptions";
			this.debugOptions.Size = new System.Drawing.Size(255, 23);
			this.debugOptions.TabIndex = 9;
			this.debugOptions.Visible = false;
			this.debugOptions.MouseLeave += new System.EventHandler(this.debugBox_MouseLeave);
			// 
			// debugOptionsLabel
			// 
			this.debugOptionsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.debugOptionsLabel.AutoSize = true;
			this.debugOptionsLabel.Location = new System.Drawing.Point(3, 3);
			this.debugOptionsLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.debugOptionsLabel.Name = "debugOptionsLabel";
			this.debugOptionsLabel.Size = new System.Drawing.Size(73, 13);
			this.debugOptionsLabel.TabIndex = 12;
			this.debugOptionsLabel.Text = "debug output:";
			// 
			// debugScrollCheck
			// 
			this.debugScrollCheck.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.debugScrollCheck.AutoSize = true;
			this.debugScrollCheck.Checked = true;
			this.debugScrollCheck.CheckState = System.Windows.Forms.CheckState.Checked;
			this.debugScrollCheck.Location = new System.Drawing.Point(178, 3);
			this.debugScrollCheck.Margin = new System.Windows.Forms.Padding(15, 3, 3, 3);
			this.debugScrollCheck.Name = "debugScrollCheck";
			this.debugScrollCheck.Size = new System.Drawing.Size(74, 17);
			this.debugScrollCheck.TabIndex = 18;
			this.debugScrollCheck.Text = "AutoScroll";
			this.debugScrollCheck.UseVisualStyleBackColor = true;
			this.debugScrollCheck.CheckedChanged += new System.EventHandler(this.debugScrollCheck_CheckedChanged);
			// 
			// debugBox
			// 
			this.debugBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.debugBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.debugBox.Font = new System.Drawing.Font("Lucida Console", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.debugBox.Location = new System.Drawing.Point(0, 0);
			this.debugBox.Name = "debugBox";
			this.debugBox.Size = new System.Drawing.Size(683, 155);
			this.debugBox.TabIndex = 8;
			this.debugBox.Text = "Welcome! Make sure NOL3 application is running and press \"Connect\"...\n";
			this.debugBox.MouseEnter += new System.EventHandler(this.debugBox_MouseEnter);
			this.debugBox.MouseLeave += new System.EventHandler(this.debugBox_MouseLeave);
			// 
			// accountsView
			// 
			this.accountsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.accTextColumn,
            this.accValueColumn,
            this.accPriceColumn,
            this.accStatusColumn});
			this.accountsView.FullRowSelect = true;
			this.accountsView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.accountsView.Location = new System.Drawing.Point(12, 41);
			this.accountsView.MultiSelect = false;
			this.accountsView.Name = "accountsView";
			this.accountsView.Size = new System.Drawing.Size(265, 240);
			this.accountsView.TabIndex = 10;
			this.accountsView.UseCompatibleStateImageBehavior = false;
			this.accountsView.View = System.Windows.Forms.View.Details;
			// 
			// instrumentsView
			// 
			this.instrumentsView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.instrumentsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.insSymbolColumn,
            this.insBidVolColumn,
            this.insBidPriceColumn,
            this.insAskPriceColumn,
            this.insAskVolColumn,
            this.insTradeVolColumn,
            this.insTradePriceColumn,
            this.insTradeTimeColumn});
			this.instrumentsView.FullRowSelect = true;
			this.instrumentsView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.instrumentsView.Location = new System.Drawing.Point(283, 41);
			this.instrumentsView.MultiSelect = false;
			this.instrumentsView.Name = "instrumentsView";
			this.instrumentsView.Size = new System.Drawing.Size(413, 240);
			this.instrumentsView.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.instrumentsView.TabIndex = 12;
			this.instrumentsView.UseCompatibleStateImageBehavior = false;
			this.instrumentsView.View = System.Windows.Forms.View.Details;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(709, 462);
			this.Controls.Add(this.instrumentsView);
			this.Controls.Add(this.accountsView);
			this.Controls.Add(this.debugPanel);
			this.Controls.Add(this.DisconnectBtn);
			this.Controls.Add(this.ConnectBtn);
			this.MinimumSize = new System.Drawing.Size(725, 330);
			this.Name = "Form1";
			this.Text = "BossaAPI .NET Class Library - GUI Test Application";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.debugPanel.ResumeLayout(false);
			this.debugPanel.PerformLayout();
			this.debugOptions.ResumeLayout(false);
			this.debugOptions.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button DisconnectBtn;
		private System.Windows.Forms.Button ConnectBtn;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Panel debugPanel;
		private System.Windows.Forms.FlowLayoutPanel debugOptions;
		private System.Windows.Forms.Label debugOptionsLabel;
		private System.Windows.Forms.CheckBox debugCheck1;
		private System.Windows.Forms.CheckBox debugCheck2;
		private System.Windows.Forms.CheckBox debugCheck3;
		private System.Windows.Forms.CheckBox debugCheck4;
		private System.Windows.Forms.CheckBox debugScrollCheck;
		private System.Windows.Forms.RichTextBox debugBox;
		private System.Windows.Forms.ListView accountsView;
		private System.Windows.Forms.ListView instrumentsView;
		private System.Windows.Forms.ColumnHeader accTextColumn;
		private System.Windows.Forms.ColumnHeader accValueColumn;
		private System.Windows.Forms.ColumnHeader accPriceColumn;
		private System.Windows.Forms.ColumnHeader accStatusColumn;
		private System.Windows.Forms.ColumnHeader insSymbolColumn;
		private System.Windows.Forms.ColumnHeader insBidVolColumn;
		private System.Windows.Forms.ColumnHeader insBidPriceColumn;
		private System.Windows.Forms.ColumnHeader insAskPriceColumn;
		private System.Windows.Forms.ColumnHeader insAskVolColumn;
		private System.Windows.Forms.ColumnHeader insTradeTimeColumn;
		private System.Windows.Forms.ColumnHeader insTradeVolColumn;
		private System.Windows.Forms.ColumnHeader insTradePriceColumn;


	}
}

