namespace VideoDedupClient.Controls.SystemInfo
{
    partial class SystemInfoCtrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DgvSystemInfo = new DataGridView();
            DgcTitle = new DataGridViewTextBoxColumn();
            DgcValue = new DataGridViewTextBoxColumn();
            tableLayoutPanel1 = new TableLayoutPanel();
            BtnCopyToClipboard = new Button();
            ((System.ComponentModel.ISupportInitialize)DgvSystemInfo).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // DgvSystemInfo
            // 
            DgvSystemInfo.AllowUserToAddRows = false;
            DgvSystemInfo.AllowUserToDeleteRows = false;
            DgvSystemInfo.AllowUserToResizeColumns = false;
            DgvSystemInfo.AllowUserToResizeRows = false;
            DgvSystemInfo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DgvSystemInfo.ColumnHeadersVisible = false;
            DgvSystemInfo.Columns.AddRange(new DataGridViewColumn[] { DgcTitle, DgcValue });
            DgvSystemInfo.Dock = DockStyle.Fill;
            DgvSystemInfo.EditMode = DataGridViewEditMode.EditProgrammatically;
            DgvSystemInfo.Location = new Point(3, 3);
            DgvSystemInfo.Name = "DgvSystemInfo";
            DgvSystemInfo.ReadOnly = true;
            DgvSystemInfo.RowHeadersVisible = false;
            DgvSystemInfo.SelectionMode = DataGridViewSelectionMode.CellSelect;
            DgvSystemInfo.ShowEditingIcon = false;
            DgvSystemInfo.Size = new Size(504, 423);
            DgvSystemInfo.TabIndex = 0;
            // 
            // DgcTitle
            // 
            DgcTitle.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            DgcTitle.HeaderText = "Title";
            DgcTitle.Name = "DgcTitle";
            DgcTitle.ReadOnly = true;
            DgcTitle.Width = 5;
            // 
            // DgcValue
            // 
            DgcValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            DgcValue.HeaderText = "Value";
            DgcValue.Name = "DgcValue";
            DgcValue.ReadOnly = true;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(BtnCopyToClipboard, 0, 1);
            tableLayoutPanel1.Controls.Add(DgvSystemInfo, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(510, 458);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // BtnCopyToClipboard
            // 
            BtnCopyToClipboard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnCopyToClipboard.Location = new Point(380, 432);
            BtnCopyToClipboard.Name = "BtnCopyToClipboard";
            BtnCopyToClipboard.Size = new Size(127, 23);
            BtnCopyToClipboard.TabIndex = 1;
            BtnCopyToClipboard.Text = "Copy To Clipboard";
            BtnCopyToClipboard.UseVisualStyleBackColor = true;
            BtnCopyToClipboard.Click += BtnCopyToClipboard_Click;
            // 
            // SystemInfoCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Name = "SystemInfoCtrl";
            Size = new Size(510, 458);
            ((System.ComponentModel.ISupportInitialize)DgvSystemInfo).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView DgvSystemInfo;
        private DataGridViewTextBoxColumn DgcTitle;
        private DataGridViewTextBoxColumn DgcValue;
        private TableLayoutPanel tableLayoutPanel1;
        private Button BtnCopyToClipboard;
    }
}
