namespace CustomSelectFileDlg
{
    partial class CustomSelectFileDialog
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
            var dataGridViewCellStyle1 = new DataGridViewCellStyle();
            var dataGridViewCellStyle2 = new DataGridViewCellStyle();
            BtnRefresh = new Button();
            BtnUp = new Button();
            BtnForward = new Button();
            BtnBack = new Button();
            BtnCancel = new Button();
            BtnOk = new Button();
            DgvContent = new DataGridView();
            DgcIcon = new DataGridViewImageColumn();
            DgcName = new DataGridViewTextBoxColumn();
            DgcDateModified = new DataGridViewTextBoxColumn();
            DgcType = new DataGridViewTextBoxColumn();
            DgcSize = new DataGridViewTextBoxColumn();
            TxtSelectedFileName = new TextBox();
            label1 = new Label();
            PabCurrentPath = new PathBox();
            CmbFilter = new ComboBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)DgvContent).BeginInit();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // BtnRefresh
            // 
            BtnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnRefresh.Image = Properties.Resources.update;
            BtnRefresh.Location = new Point(564, 12);
            BtnRefresh.Name = "BtnRefresh";
            BtnRefresh.Size = new Size(37, 27);
            BtnRefresh.TabIndex = 8;
            BtnRefresh.UseVisualStyleBackColor = true;
            BtnRefresh.Click += HandleBtnRefreshClick;
            // 
            // BtnUp
            // 
            BtnUp.Image = Properties.Resources.bullet_arrow_up;
            BtnUp.Location = new Point(98, 12);
            BtnUp.Name = "BtnUp";
            BtnUp.Size = new Size(37, 27);
            BtnUp.TabIndex = 6;
            BtnUp.UseVisualStyleBackColor = true;
            BtnUp.Click += HandleBtnUpClick;
            // 
            // BtnForward
            // 
            BtnForward.Enabled = false;
            BtnForward.Image = Properties.Resources.bullet_arrow_right;
            BtnForward.Location = new Point(55, 12);
            BtnForward.Name = "BtnForward";
            BtnForward.Size = new Size(37, 27);
            BtnForward.TabIndex = 5;
            BtnForward.UseVisualStyleBackColor = true;
            BtnForward.Click += HandleBtnForwardClick;
            // 
            // BtnBack
            // 
            BtnBack.Enabled = false;
            BtnBack.Image = Properties.Resources.bullet_arrow_left;
            BtnBack.Location = new Point(12, 12);
            BtnBack.Name = "BtnBack";
            BtnBack.Size = new Size(37, 27);
            BtnBack.TabIndex = 4;
            BtnBack.UseVisualStyleBackColor = true;
            BtnBack.Click += HandleBtnBackClick;
            // 
            // BtnCancel
            // 
            BtnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Location = new Point(526, 327);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(75, 23);
            BtnCancel.TabIndex = 3;
            BtnCancel.Text = "&Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOk
            // 
            BtnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BtnOk.Location = new Point(445, 327);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new Size(75, 23);
            BtnOk.TabIndex = 2;
            BtnOk.Text = "&OK";
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += HandleBtnOkClick;
            // 
            // DgvContent
            // 
            DgvContent.AllowUserToAddRows = false;
            DgvContent.AllowUserToDeleteRows = false;
            DgvContent.AllowUserToResizeRows = false;
            DgvContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DgvContent.BackgroundColor = SystemColors.Window;
            DgvContent.CellBorderStyle = DataGridViewCellBorderStyle.None;
            DgvContent.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            DgvContent.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            DgvContent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            DgvContent.Columns.AddRange(new DataGridViewColumn[] { DgcIcon, DgcName, DgcDateModified, DgcType, DgcSize });
            DgvContent.EditMode = DataGridViewEditMode.EditProgrammatically;
            DgvContent.EnableHeadersVisualStyles = false;
            DgvContent.Location = new Point(12, 45);
            DgvContent.MultiSelect = false;
            DgvContent.Name = "DgvContent";
            DgvContent.RowHeadersVisible = false;
            DgvContent.RowTemplate.Height = 25;
            DgvContent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DgvContent.ShowCellToolTips = false;
            DgvContent.ShowEditingIcon = false;
            DgvContent.Size = new Size(589, 247);
            DgvContent.TabIndex = 0;
            DgvContent.CellDoubleClick += HandleDgvContentCellDoubleClick;
            DgvContent.SelectionChanged += HandleDgvContentSelectionChanged;
            DgvContent.SortCompare += HandleDgvContentSortCompare;
            DgvContent.KeyDown += HandleDgvContentKeyDown;
            DgvContent.MouseDown += HandleDgvContentMouseDown;
            // 
            // DgcIcon
            // 
            DgcIcon.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = null;
            DgcIcon.DefaultCellStyle = dataGridViewCellStyle2;
            DgcIcon.Frozen = true;
            DgcIcon.HeaderText = "";
            DgcIcon.MinimumWidth = 10;
            DgcIcon.Name = "DgcIcon";
            DgcIcon.Width = 10;
            // 
            // DgcName
            // 
            DgcName.Frozen = true;
            DgcName.HeaderText = "Name";
            DgcName.Name = "DgcName";
            DgcName.Width = 300;
            // 
            // DgcDateModified
            // 
            DgcDateModified.Frozen = true;
            DgcDateModified.HeaderText = "Date Modified";
            DgcDateModified.Name = "DgcDateModified";
            DgcDateModified.Width = 120;
            // 
            // DgcType
            // 
            DgcType.Frozen = true;
            DgcType.HeaderText = "Type";
            DgcType.Name = "DgcType";
            // 
            // DgcSize
            // 
            DgcSize.Frozen = true;
            DgcSize.HeaderText = "Size";
            DgcSize.Name = "DgcSize";
            // 
            // TxtSelectedFileName
            // 
            TxtSelectedFileName.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TxtSelectedFileName.Location = new Point(61, 0);
            TxtSelectedFileName.Margin = new Padding(0);
            TxtSelectedFileName.Name = "TxtSelectedFileName";
            TxtSelectedFileName.Size = new Size(369, 23);
            TxtSelectedFileName.TabIndex = 1;
            TxtSelectedFileName.TextChanged += HandleTxtSelectedFileNameTextChanged;
            TxtSelectedFileName.KeyDown += HandleTxtSelectedFileNameKeyDown;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(0, 4);
            label1.Margin = new Padding(0);
            label1.Name = "label1";
            label1.Size = new Size(61, 15);
            label1.TabIndex = 16;
            label1.Text = "File name:";
            // 
            // PabCurrentPath
            // 
            PabCurrentPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            PabCurrentPath.BorderStyle = BorderStyle.FixedSingle;
            PabCurrentPath.Location = new Point(141, 12);
            PabCurrentPath.MaximumSize = new Size(999999999, 923);
            PabCurrentPath.MinimumSize = new Size(0, 27);
            PabCurrentPath.Name = "PabCurrentPath";
            PabCurrentPath.Size = new Size(417, 27);
            PabCurrentPath.TabIndex = 17;
            PabCurrentPath.CurrentPathChanged += HandlePabCurrentPathCurrentPathChanged;
            // 
            // CmbFilter
            // 
            CmbFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbFilter.FormattingEnabled = true;
            CmbFilter.Location = new Point(433, 0);
            CmbFilter.Margin = new Padding(3, 0, 0, 0);
            CmbFilter.Name = "CmbFilter";
            CmbFilter.Size = new Size(156, 23);
            CmbFilter.TabIndex = 18;
            CmbFilter.SelectedIndexChanged += HandleCmbFilterSelectedIndexChanged;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(CmbFilter, 2, 0);
            tableLayoutPanel1.Controls.Add(TxtSelectedFileName, 1, 0);
            tableLayoutPanel1.Location = new Point(12, 298);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(589, 23);
            tableLayoutPanel1.TabIndex = 19;
            // 
            // CustomSelectFileDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(613, 362);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(PabCurrentPath);
            Controls.Add(DgvContent);
            Controls.Add(BtnRefresh);
            Controls.Add(BtnUp);
            Controls.Add(BtnForward);
            Controls.Add(BtnBack);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOk);
            KeyPreview = true;
            Name = "CustomSelectFileDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "CustomSelectFileDialog";
            KeyDown += HandleCustomSelectFileDialogKeyDown;
            ((System.ComponentModel.ISupportInitialize)DgvContent).EndInit();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button BtnRefresh;
        private Button BtnUp;
        private Button BtnForward;
        private Button BtnBack;
        private Button BtnCancel;
        private Button BtnOk;
        private DataGridView DgvContent;
        private TextBox TxtSelectedFileName;
        private Label label1;
        private DataGridViewImageColumn DgcIcon;
        private DataGridViewTextBoxColumn DgcName;
        private DataGridViewTextBoxColumn DgcDateModified;
        private DataGridViewTextBoxColumn DgcType;
        private DataGridViewTextBoxColumn DgcSize;
        private PathBox PabCurrentPath;
        private ComboBox CmbFilter;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
