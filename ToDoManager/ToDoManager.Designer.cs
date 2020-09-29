namespace ToDoManager
{
    partial class ToDoManager
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
            this.BtnCancel = new System.Windows.Forms.Button();
            this.BtnOkay = new System.Windows.Forms.Button();
            this.DgvToDoList = new System.Windows.Forms.DataGridView();
            this.DgcDone = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DgcText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DgcDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ChbShowCompletedItems = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TxtToDoFilePath = new System.Windows.Forms.TextBox();
            this.BtnToDoFileSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DgvToDoList)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnCancel
            // 
            this.BtnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnCancel.Location = new System.Drawing.Point(713, 415);
            this.BtnCancel.Name = "BtnCancel";
            this.BtnCancel.Size = new System.Drawing.Size(75, 23);
            this.BtnCancel.TabIndex = 17;
            this.BtnCancel.Text = "Cancel";
            this.BtnCancel.UseVisualStyleBackColor = true;
            // 
            // BtnOkay
            // 
            this.BtnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnOkay.Location = new System.Drawing.Point(632, 415);
            this.BtnOkay.Name = "BtnOkay";
            this.BtnOkay.Size = new System.Drawing.Size(75, 23);
            this.BtnOkay.TabIndex = 16;
            this.BtnOkay.Text = "OK";
            this.BtnOkay.UseVisualStyleBackColor = true;
            this.BtnOkay.Click += new System.EventHandler(this.BtnOkay_Click);
            // 
            // DgvToDoList
            // 
            this.DgvToDoList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DgvToDoList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvToDoList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DgcDone,
            this.DgcText,
            this.DgcDelete});
            this.DgvToDoList.Location = new System.Drawing.Point(12, 37);
            this.DgvToDoList.MultiSelect = false;
            this.DgvToDoList.Name = "DgvToDoList";
            this.DgvToDoList.RowHeadersVisible = false;
            this.DgvToDoList.Size = new System.Drawing.Size(776, 372);
            this.DgvToDoList.TabIndex = 18;
            this.DgvToDoList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvToDoList_CellContentClick);
            this.DgvToDoList.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.DgvToDoList_DefaultValuesNeeded);
            // 
            // DgcDone
            // 
            this.DgcDone.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DgcDone.HeaderText = "Done";
            this.DgcDone.Name = "DgcDone";
            this.DgcDone.Width = 39;
            // 
            // DgcText
            // 
            this.DgcText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DgcText.HeaderText = "Text";
            this.DgcText.Name = "DgcText";
            // 
            // DgcDelete
            // 
            this.DgcDelete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.DgcDelete.HeaderText = "Delete";
            this.DgcDelete.Name = "DgcDelete";
            this.DgcDelete.Width = 44;
            // 
            // ChbShowCompletedItems
            // 
            this.ChbShowCompletedItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ChbShowCompletedItems.AutoSize = true;
            this.ChbShowCompletedItems.Location = new System.Drawing.Point(12, 419);
            this.ChbShowCompletedItems.Name = "ChbShowCompletedItems";
            this.ChbShowCompletedItems.Size = new System.Drawing.Size(132, 17);
            this.ChbShowCompletedItems.TabIndex = 19;
            this.ChbShowCompletedItems.Text = "Show completed items";
            this.ChbShowCompletedItems.UseVisualStyleBackColor = true;
            this.ChbShowCompletedItems.CheckedChanged += new System.EventHandler(this.ChbShowCompletedItems_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "ToDo-File:";
            // 
            // TxtToDoFilePath
            // 
            this.TxtToDoFilePath.Location = new System.Drawing.Point(75, 10);
            this.TxtToDoFilePath.Name = "TxtToDoFilePath";
            this.TxtToDoFilePath.Size = new System.Drawing.Size(678, 20);
            this.TxtToDoFilePath.TabIndex = 21;
            // 
            // BtnToDoFileSelect
            // 
            this.BtnToDoFileSelect.Location = new System.Drawing.Point(759, 8);
            this.BtnToDoFileSelect.Name = "BtnToDoFileSelect";
            this.BtnToDoFileSelect.Size = new System.Drawing.Size(29, 23);
            this.BtnToDoFileSelect.TabIndex = 22;
            this.BtnToDoFileSelect.Text = "...";
            this.BtnToDoFileSelect.UseVisualStyleBackColor = true;
            this.BtnToDoFileSelect.Click += new System.EventHandler(this.BtnToDoFileSelect_Click);
            // 
            // ToDoManager
            // 
            this.AcceptButton = this.BtnOkay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnCancel;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BtnToDoFileSelect);
            this.Controls.Add(this.TxtToDoFilePath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ChbShowCompletedItems);
            this.Controls.Add(this.DgvToDoList);
            this.Controls.Add(this.BtnCancel);
            this.Controls.Add(this.BtnOkay);
            this.Name = "ToDoManager";
            this.Text = "ToDo Manager";
            ((System.ComponentModel.ISupportInitialize)(this.DgvToDoList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnCancel;
        private System.Windows.Forms.Button BtnOkay;
        private System.Windows.Forms.DataGridView DgvToDoList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DgcDone;
        private System.Windows.Forms.DataGridViewTextBoxColumn DgcText;
        private System.Windows.Forms.DataGridViewButtonColumn DgcDelete;
        private System.Windows.Forms.CheckBox ChbShowCompletedItems;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TxtToDoFilePath;
        private System.Windows.Forms.Button BtnToDoFileSelect;
    }
}
