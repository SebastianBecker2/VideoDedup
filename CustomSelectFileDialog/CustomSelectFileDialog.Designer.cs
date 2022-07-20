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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.BtnUp = new System.Windows.Forms.Button();
            this.BtnForward = new System.Windows.Forms.Button();
            this.BtnBack = new System.Windows.Forms.Button();
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOk = new System.Windows.Forms.Button();
            this.DgvContent = new System.Windows.Forms.DataGridView();
            this.DgcIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.DgcName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgcDateModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgcType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgcSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TxtSelectedFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PabCurrentPath = new CustomSelectFileDlg.PathBox();
            ((System.ComponentModel.ISupportInitialize)(this.DgvContent)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefresh.Image = global::CustomSelectFileDlg.Properties.Resources.update;
            this.BtnRefresh.Location = new System.Drawing.Point(566, 12);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(37, 27);
            this.BtnRefresh.TabIndex = 8;
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.HandleBtnRefreshClick);
            // 
            // BtnUp
            // 
            this.BtnUp.Image = global::CustomSelectFileDlg.Properties.Resources.bullet_arrow_up;
            this.BtnUp.Location = new System.Drawing.Point(98, 12);
            this.BtnUp.Name = "BtnUp";
            this.BtnUp.Size = new System.Drawing.Size(37, 27);
            this.BtnUp.TabIndex = 6;
            this.BtnUp.UseVisualStyleBackColor = true;
            this.BtnUp.Click += new System.EventHandler(this.HandleBtnUpClick);
            // 
            // BtnForward
            // 
            this.BtnForward.Enabled = false;
            this.BtnForward.Image = global::CustomSelectFileDlg.Properties.Resources.bullet_arrow_right;
            this.BtnForward.Location = new System.Drawing.Point(55, 12);
            this.BtnForward.Name = "BtnForward";
            this.BtnForward.Size = new System.Drawing.Size(37, 27);
            this.BtnForward.TabIndex = 5;
            this.BtnForward.UseVisualStyleBackColor = true;
            this.BtnForward.Click += new System.EventHandler(this.HandleBtnForwardClick);
            // 
            // BtnBack
            // 
            this.BtnBack.Enabled = false;
            this.BtnBack.Image = global::CustomSelectFileDlg.Properties.Resources.bullet_arrow_left;
            this.BtnBack.Location = new System.Drawing.Point(12, 12);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(37, 27);
            this.BtnBack.TabIndex = 4;
            this.BtnBack.UseVisualStyleBackColor = true;
            this.BtnBack.Click += new System.EventHandler(this.HandleBtnBackClick);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(528, 322);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 3;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOk
            // 
            this.BtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOk.Location = new System.Drawing.Point(447, 322);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(75, 23);
            this.BtnOk.TabIndex = 2;
            this.BtnOk.Text = "&OK";
            this.BtnOk.UseVisualStyleBackColor = true;
            this.BtnOk.Click += new System.EventHandler(this.HandleBtnOkClick);
            // 
            // DgvContent
            // 
            this.DgvContent.AllowUserToAddRows = false;
            this.DgvContent.AllowUserToDeleteRows = false;
            this.DgvContent.AllowUserToResizeRows = false;
            this.DgvContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvContent.BackgroundColor = System.Drawing.SystemColors.Window;
            this.DgvContent.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.DgvContent.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DgvContent.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DgvContent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DgvContent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgcIcon,
            this.DgcName,
            this.DgcDateModified,
            this.DgcType,
            this.DgcSize});
            this.DgvContent.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DgvContent.EnableHeadersVisualStyles = false;
            this.DgvContent.Location = new System.Drawing.Point(12, 45);
            this.DgvContent.MultiSelect = false;
            this.DgvContent.Name = "DgvContent";
            this.DgvContent.RowHeadersVisible = false;
            this.DgvContent.RowTemplate.Height = 25;
            this.DgvContent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvContent.ShowCellToolTips = false;
            this.DgvContent.ShowEditingIcon = false;
            this.DgvContent.Size = new System.Drawing.Size(591, 271);
            this.DgvContent.TabIndex = 0;
            this.DgvContent.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleDgvContentCellDoubleClick);
            this.DgvContent.SelectionChanged += new System.EventHandler(this.HandleDgvContentSelectionChanged);
            this.DgvContent.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.HandleDgvContentSortCompare);
            this.DgvContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleDgvContentKeyDown);
            this.DgvContent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HandleDgvContentMouseDown);
            // 
            // DgcIcon
            // 
            this.DgcIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = null;
            this.DgcIcon.DefaultCellStyle = dataGridViewCellStyle2;
            this.DgcIcon.Frozen = true;
            this.DgcIcon.HeaderText = "";
            this.DgcIcon.MinimumWidth = 10;
            this.DgcIcon.Name = "DgcIcon";
            this.DgcIcon.Width = 10;
            // 
            // DgcName
            // 
            this.DgcName.Frozen = true;
            this.DgcName.HeaderText = "Name";
            this.DgcName.Name = "DgcName";
            this.DgcName.Width = 300;
            // 
            // DgcDateModified
            // 
            this.DgcDateModified.Frozen = true;
            this.DgcDateModified.HeaderText = "Date Modified";
            this.DgcDateModified.Name = "DgcDateModified";
            this.DgcDateModified.Width = 120;
            // 
            // DgcType
            // 
            this.DgcType.Frozen = true;
            this.DgcType.HeaderText = "Type";
            this.DgcType.Name = "DgcType";
            // 
            // DgcSize
            // 
            this.DgcSize.Frozen = true;
            this.DgcSize.HeaderText = "Size";
            this.DgcSize.Name = "DgcSize";
            // 
            // TxtSelectedFileName
            // 
            this.TxtSelectedFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSelectedFileName.Location = new System.Drawing.Point(79, 322);
            this.TxtSelectedFileName.Name = "TxtSelectedFileName";
            this.TxtSelectedFileName.Size = new System.Drawing.Size(362, 23);
            this.TxtSelectedFileName.TabIndex = 1;
            this.TxtSelectedFileName.TextChanged += new System.EventHandler(this.HandleTxtSelectedFileNameTextChanged);
            this.TxtSelectedFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleTxtSelectedFileNameKeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 326);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "File name:";
            // 
            // PabCurrentPath
            // 
            this.PabCurrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PabCurrentPath.Location = new System.Drawing.Point(141, 12);
            this.PabCurrentPath.MaximumSize = new System.Drawing.Size(999999999, 923);
            this.PabCurrentPath.Name = "PabCurrentPath";
            this.PabCurrentPath.Size = new System.Drawing.Size(419, 27);
            this.PabCurrentPath.TabIndex = 17;
            this.PabCurrentPath.CurrentPathChanged += new System.EventHandler<CustomSelectFileDlg.EventArgs.CurrentPathChangedEventArgs>(this.HandlePabCurrentPathCurrentPathChanged);
            // 
            // CustomSelectFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(615, 357);
            this.Controls.Add(this.PabCurrentPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtSelectedFileName);
            this.Controls.Add(this.DgvContent);
            this.Controls.Add(this.BtnRefresh);
            this.Controls.Add(this.BtnUp);
            this.Controls.Add(this.BtnForward);
            this.Controls.Add(this.BtnBack);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.KeyPreview = true;
            this.Name = "CustomSelectFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CustomSelectFileDialog";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleCustomSelectFileDialogKeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.DgvContent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}
