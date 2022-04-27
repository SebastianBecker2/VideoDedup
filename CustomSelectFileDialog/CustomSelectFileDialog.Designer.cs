namespace CustomSelectFileDialog
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
            this.BtnRefresh = new System.Windows.Forms.Button();
            this.TxtCurrentPath = new System.Windows.Forms.TextBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.DgvContent)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnRefresh
            // 
            this.BtnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnRefresh.Image = global::CustomSelectFileDialog.Properties.Resources.update;
            this.BtnRefresh.Location = new System.Drawing.Point(751, 12);
            this.BtnRefresh.Name = "BtnRefresh";
            this.BtnRefresh.Size = new System.Drawing.Size(37, 23);
            this.BtnRefresh.TabIndex = 13;
            this.BtnRefresh.UseVisualStyleBackColor = true;
            this.BtnRefresh.Click += new System.EventHandler(this.HandleBtnRefreshClick);
            // 
            // TxtCurrentPath
            // 
            this.TxtCurrentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtCurrentPath.Location = new System.Drawing.Point(141, 12);
            this.TxtCurrentPath.Name = "TxtCurrentPath";
            this.TxtCurrentPath.Size = new System.Drawing.Size(604, 23);
            this.TxtCurrentPath.TabIndex = 12;
            this.TxtCurrentPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleTxtCurrentPathKeyDown);
            // 
            // BtnUp
            // 
            this.BtnUp.Image = global::CustomSelectFileDialog.Properties.Resources.bullet_arrow_up;
            this.BtnUp.Location = new System.Drawing.Point(98, 12);
            this.BtnUp.Name = "BtnUp";
            this.BtnUp.Size = new System.Drawing.Size(37, 23);
            this.BtnUp.TabIndex = 11;
            this.BtnUp.UseVisualStyleBackColor = true;
            this.BtnUp.Click += new System.EventHandler(this.HandleBtnUpClick);
            // 
            // BtnForward
            // 
            this.BtnForward.Enabled = false;
            this.BtnForward.Image = global::CustomSelectFileDialog.Properties.Resources.bullet_arrow_right;
            this.BtnForward.Location = new System.Drawing.Point(55, 12);
            this.BtnForward.Name = "BtnForward";
            this.BtnForward.Size = new System.Drawing.Size(37, 23);
            this.BtnForward.TabIndex = 10;
            this.BtnForward.UseVisualStyleBackColor = true;
            this.BtnForward.Click += new System.EventHandler(this.BtnForward_Click);
            // 
            // BtnBack
            // 
            this.BtnBack.Enabled = false;
            this.BtnBack.Image = global::CustomSelectFileDialog.Properties.Resources.bullet_arrow_left;
            this.BtnBack.Location = new System.Drawing.Point(12, 12);
            this.BtnBack.Name = "BtnBack";
            this.BtnBack.Size = new System.Drawing.Size(37, 23);
            this.BtnBack.TabIndex = 9;
            this.BtnBack.UseVisualStyleBackColor = true;
            this.BtnBack.Click += new System.EventHandler(this.HandleBtnBackClick);
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(713, 415);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 8;
            this.BtnCancel.Text = "&Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOk
            // 
            this.BtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOk.Location = new System.Drawing.Point(632, 415);
            this.BtnOk.Name = "BtnOk";
            this.BtnOk.Size = new System.Drawing.Size(75, 23);
            this.BtnOk.TabIndex = 7;
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
            this.DgvContent.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.DgvContent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgcIcon,
            this.DgcName,
            this.DgcDateModified,
            this.DgcType,
            this.DgcSize});
            this.DgvContent.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DgvContent.Location = new System.Drawing.Point(12, 41);
            this.DgvContent.MultiSelect = false;
            this.DgvContent.Name = "DgvContent";
            this.DgvContent.RowHeadersVisible = false;
            this.DgvContent.RowTemplate.Height = 25;
            this.DgvContent.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DgvContent.ShowEditingIcon = false;
            this.DgvContent.Size = new System.Drawing.Size(776, 368);
            this.DgvContent.TabIndex = 14;
            this.DgvContent.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.HandleDgvContentCellDoubleClick);
            this.DgvContent.SelectionChanged += new System.EventHandler(this.HandleDgvContentSelectionChanged);
            this.DgvContent.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleDgvContentKeyDown);
            // 
            // DgcIcon
            // 
            this.DgcIcon.Frozen = true;
            this.DgcIcon.HeaderText = "";
            this.DgcIcon.Name = "DgcIcon";
            this.DgcIcon.Width = 20;
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
            this.TxtSelectedFileName.Location = new System.Drawing.Point(79, 415);
            this.TxtSelectedFileName.Name = "TxtSelectedFileName";
            this.TxtSelectedFileName.Size = new System.Drawing.Size(547, 23);
            this.TxtSelectedFileName.TabIndex = 15;
            this.TxtSelectedFileName.TextChanged += new System.EventHandler(this.HandleTxtSelectedFileNameTextChanged);
            this.TxtSelectedFileName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleTxtSelectedFileNameKeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "File name:";
            // 
            // CustomSelectFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TxtSelectedFileName);
            this.Controls.Add(this.DgvContent);
            this.Controls.Add(this.BtnRefresh);
            this.Controls.Add(this.TxtCurrentPath);
            this.Controls.Add(this.BtnUp);
            this.Controls.Add(this.BtnForward);
            this.Controls.Add(this.BtnBack);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOk);
            this.Name = "CustomSelectFileDialog";
            this.Text = "CustomSelectFileDialog";
            ((System.ComponentModel.ISupportInitialize)(this.DgvContent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button BtnRefresh;
        private TextBox TxtCurrentPath;
        private Button BtnUp;
        private Button BtnForward;
        private Button BtnBack;
        private Button BtnCancel;
        private Button BtnOk;
        private DataGridView DgvContent;
        private DataGridViewImageColumn DgcIcon;
        private DataGridViewTextBoxColumn DgcName;
        private DataGridViewTextBoxColumn DgcDateModified;
        private DataGridViewTextBoxColumn DgcType;
        private DataGridViewTextBoxColumn DgcSize;
        private TextBox TxtSelectedFileName;
        private Label label1;
    }
}